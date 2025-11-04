using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BattleManager : NetworkBehaviour
{
    private List<ABaseUnit> battleUnits = new List<ABaseUnit>();

    [Header("Positioning")]
    [SerializeField] private List<Transform> enemySpawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    [Header("Enemy Spawning")]
    [SerializeField] private List<SpawnableEnemy> spawnableEnemies = new List<SpawnableEnemy>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [Rpc(SendTo.Server)]
    public void BattleStartedRPC()
    {
        // add all party members to battle units, then enemies
        battleUnits.Clear();
        
        for (int i = 0; i < PartyManager.Singleton.GetAlivePartyMemberCount(); i++)
        {
            battleUnits.Add(PartyManager.Singleton.GetPartyMember(i));
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