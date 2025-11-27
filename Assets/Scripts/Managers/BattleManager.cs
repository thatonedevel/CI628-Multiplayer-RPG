using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class BattleManager : NetworkBehaviour
{
    // have an available singleton on the server
    public static BattleManager Singleton;

    private List<ABaseUnit> battleUnits = new List<ABaseUnit>();

    [Header("Positioning")]
    [SerializeField] private List<Transform> enemySpawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    [Header("Enemy Spawning")]
    [SerializeField] private List<SpawnableEnemy> spawnableEnemies = new List<SpawnableEnemy>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Singleton != null)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            if (IsServer || IsHost)
            {
                Singleton = this;
                BattleStarted();
            }
        }
    }

    //[Rpc(SendTo.Server)]
    public void BattleStarted()
    {
        Debug.Log("SERVER: Starting battle");
        // add all party members to battle units, then enemies
        battleUnits.Clear();
        
        for (int i = 0; i < PartyManager.Singleton.GetAlivePartyMemberCount(); i++)
        {
            battleUnits.Add(PartyManager.Singleton.GetPartyMember(i));
            // set position of object
            PartyManager.Singleton.GetPartyMember(i).transform.position = playerSpawnPositions[i].position;
        }

        // add enemies. start with detected enemy and then add others
        // roll enemy count, spawn amount that is proportionate to party size

        int maxEnemies = PartyManager.Singleton.GetAlivePartyMemberCount() == 4 ? 4 : 3;

        int enemyCount = Random.Range(1, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            // spawn a zombie for now
            // TODO: tweak this once more enemy types are added
            GameObject zombieObj = Instantiate(spawnableEnemies[0].enemyPrefab, enemySpawnPositions[i].position, Quaternion.Euler(0, 180, 0));

            zombieObj.GetComponent<NetworkObject>().Spawn(true); // destroy once scene is left

            // add unit component to battle units list
            battleUnits.Add(zombieObj.GetComponent<ABaseUnit>());
        }

        Debug.Log("Battle Started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnTaken(ulong unitID)
    {
        if (IsServer || IsHost)
        {
            
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
}

public enum BattleState
{
    INIT,
    PLAYER_TURN,
    ENEMY_TURN
}

[System.Serializable]
public class SpawnableEnemy
{
    public GameObject enemyPrefab;
    public int spawnWeight;
    public string enemyName;
}