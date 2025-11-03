using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BattleManager : NetworkBehaviour
{
    private List<ABaseUnit> battleUnits = new List<ABaseUnit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [Rpc(SendTo.Server)]
    public void BattleStartedRPC()
    {

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