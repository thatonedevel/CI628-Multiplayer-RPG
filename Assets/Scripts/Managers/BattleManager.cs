using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BattleManager : NetworkBehaviour
{
    // have an available singleton on the server
    public static BattleManager Singleton;

    // battle events
    public static event Action<int, ulong, bool> OnCompletedTurnEndProcessing;
    public static event Action<ulong> OnTurnEnd;
    public static event Action OnTurnStarted;


    // lists for referencing each unit type
    // have separate lists for enemy and player units for specific unit management stuff
    // battleunits list is primarily used for turn tracking
    private List<ABaseUnit> battleUnits = new();

    private List<PlayerUnit> playerUnits = new();
    private List<PlayerUnit> deceasedPlayerUnits = new();

    private List<BasicEnemy> enemyUnits = new();
    

    [Header("Positioning")]
    [SerializeField] private List<Transform> enemySpawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    [Header("Enemy Spawning")]
    [SerializeField] private List<SpawnableEnemy> spawnableEnemies = new List<SpawnableEnemy>();

    [Header("Misc")]
    [SerializeField] private BattleUIController uiController;


    // internal info
    private int turnIndex = 0; // tracking current turn
    private int experiencePool = 0; // experience to be given to players when battle is won

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Singleton != null)
        {
            NetworkObject.Despawn();
        }
        else
        {
            if (IsServer)
            {

                Singleton = this;

                // subscribe to turn end events
                ABaseUnit.TurnTakenEvent += OnTurnTaken;
                BasicEnemy.EnemyKilledEvent += EnemyDeathListener;
                PlayerUnit.OnPlayerKilled += PlayerDeathListener;
                OnTurnEnd += EndTurn;


                BattleStarted();
            }
        }
    }

    //[Rpc(SendTo.Server)]
    public void BattleStarted()
    {
        Debug.Log("SERVER: Starting battle");
        
        // reset each list for safety reasons
        battleUnits.Clear();
        playerUnits.Clear();
        enemyUnits.Clear();
        
        // add all party members to battle units, then enemies
        for (int i = 0; i < PartyManager.Singleton.GetAlivePartyMemberCount(); i++)
        {
            battleUnits.Add(PartyManager.Singleton.GetPartyMember(i));
            // set position of object
            Debug.Log("SERVER: Setting positions");
            PartyManager.Singleton.GetPartyMember(i).transform.position = playerSpawnPositions[i].position;
            // add all pary members to player list
            playerUnits.Add(PartyManager.Singleton.GetPartyMember(i));

            // disable the individual cameras
            playerUnits[i].GetComponent<NetPlayerMovement>().DisableOverworldCameraRPC();
        }

        // add enemies. start with detected enemy and then add others
        // roll enemy count, spawn amount that is proportionate to party size

        int maxEnemies = PartyManager.Singleton.GetAlivePartyMemberCount() == 4 ? 4 : 3;

        int enemyCount = UnityEngine.Random.Range(1, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            // spawn a zombie for now
            // TODO: tweak this once more enemy types are added
            GameObject zombieObj = Instantiate(spawnableEnemies[0].enemyPrefab, enemySpawnPositions[i].position, Quaternion.Euler(0, 180, 0));

            zombieObj.GetComponent<NetworkObject>().Spawn(true); // destroy once scene is left

            // add unit component to battle units list
            battleUnits.Add(zombieObj.GetComponent<BasicEnemy>());
            // add the unit to the enemies list
            enemyUnits.Add(zombieObj.GetComponent<BasicEnemy>());
        }

        Debug.Log("SERVER: Battle Started");

        // activate the gui
        uiController.ActivationRPC(playerUnits[0].NetworkObjectId);
    }

    public void OnTurnTaken(ulong unitID)
    {
        if (IsServer || IsHost)
        {
            EndTurn(unitID);
        }
    }

    public ulong[] GetAllEnemyUnits()
    {
        // server only
        if (!IsServer && !IsHost) 
            throw new NotServerException("Hey dummy you need to be a server to call this");

        List<ulong> enemyUnitList = new List<ulong>();

        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] is BasicEnemy)
            {
                // get network id, add to list
                ulong id = battleUnits[i].GetComponent<NetworkObject>().NetworkObjectId;
                enemyUnitList.Add(id);
            }
        }

        return enemyUnitList.ToArray();
    }

    public void EnemyAttacked(int enemyIndex)
    {
        // enemy damage logic here
        if (!IsServer)
            return;

        Debug.Log("SERVER: Enemy has been attacked");

        // grab attack from player
        if (battleUnits[turnIndex] is not PlayerUnit)
            return;
        int atk = battleUnits[turnIndex].GetComponent<PlayerUnit>().attack.Value;

        Debug.Log("SERVER: Enemy was damaged");
        enemyUnits[enemyIndex].TakeDamage(atk);

        // end the turn
        OnTurnEnd?.Invoke(battleUnits[turnIndex].GetComponent<NetworkObject>().NetworkObjectId);
    }

    private void EndTurn(ulong id)
    {
        if (!IsServer)
            return;

        // ran via event invocation

        Debug.Log("SERVER: Ending turn");

        // calculate next turn index (i.e. do we need to loop back round?)
        int nextTurnIndex = turnIndex >= battleUnits.Count - 1 ? 0 : turnIndex + 1;

        bool isPlayerUnit = battleUnits[nextTurnIndex] is PlayerUnit;

        // fire turn ended event
        OnCompletedTurnEndProcessing?.Invoke(turnIndex, id, isPlayerUnit);

        // update turn index
        turnIndex = nextTurnIndex;

        // check if the next turn is a player or an enemy
        if (!isPlayerUnit)
        {
            // handle the enemy turn
            HandleEnemyTurn();
        }
        else
        {
            // is player, enable that player's ui
            var unit = battleUnits[nextTurnIndex] as PlayerUnit;
            uiController.EnableBattleUIRPC(unit.NetworkObjectId);
        }
    }

    private void HandleEnemyTurn()
    {
        if (!IsServer) 
            return;
        // grab current enemy
        BasicEnemy currentEnemy = battleUnits[turnIndex] as BasicEnemy;

        // call its attack method
        currentEnemy.AttackPlayer();
        
    }

    // enemy and player death event handlers
    private void EnemyDeathListener(ulong enemyID)
    {
        if (!IsServer)
            return;

        // enemy death detected
        // get the gameobject from the given id
        BasicEnemy target = null;

        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (enemyUnits[i].NetworkBehaviourId == enemyID)
            {
                target = enemyUnits[i];
                break;
            }
        }

        if (target is not null)
        {
            // remove the target from each array, and then despawn it
            enemyUnits.Remove(target);
            battleUnits.Remove(target);

            // add xp to pool
            experiencePool += target.experienceDrop;

            // despawn enemy
            target.NetworkObject.Despawn();
        }

        // once done, check enemy array size
        if (enemyUnits.Count == 0)
        {
            // players one the battle :D
            EndBattle();
        }
    }

    private void PlayerDeathListener(ulong playerID)
    {
        if (!IsServer) 
            return;

        // similar logic to the enemy death, but with some differences
        Debug.Log("SERVER: Player: " + " was killed");

        // get the obj of dead player
        PlayerUnit deceasedPlayer = null;

        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (playerUnits[i].NetworkObjectId == playerID)
            {
                deceasedPlayer = playerUnits[i];
                break;
            }
        }

        if (deceasedPlayer is not null)
        {
            // update player state to spectating
            deceasedPlayer.UpdatePlayerStateRPC(PlayerState.SPECTATOR);
            // move player to the "knock-out" list so they can potentially be revived
            // also remove them from the other lists (as those track alive units)
            battleUnits.Remove(deceasedPlayer);
            playerUnits.Remove(deceasedPlayer);

            deceasedPlayerUnits.Add(deceasedPlayer);

            // check if all player units are dead
            if (playerUnits.Count == 0)
            {
                EndBattle(false);
            }
        }
    }

    private void EndBattle(bool didPartyWin=true)
    {
        if (didPartyWin)
        {
            // TODO: on the one hand, gold
            // for now, just return to the overworld
            GameController.Singleton.TransitionToOverworld();
        }
        else
        {
            // TODO: on the other hand, horrible agonising failure /ref
            // call gameover method on game controller
        }
    }
}

[System.Serializable]
public class SpawnableEnemy
{
    public GameObject enemyPrefab;
    public int spawnWeight;
    public string enemyName;
}

public enum TargetType
{
    PLAYER,
    ENEMY,
    NULL
}