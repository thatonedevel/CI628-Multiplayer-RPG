using System;
using UnityEngine;
using Unity.Netcode;

public class BasicEnemy : ABaseUnit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // enemy-specific events
    public static event Action<ulong> EnemyDamagedEvent;
    public static event Action<ulong> EnemyKilledEvent;

    // enemy only stats
    public int experienceDrop = 0;
    public int goldDrop = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(int damageAmount)
    {
        throw new System.NotImplementedException();
    }

    public virtual void AttackPlayer()
    {
        if (IsServer)
        {
            // enemy attack logic
            // pick a player to attack
            int index = UnityEngine.Random.Range(0, PartyManager.Singleton.GetAlivePartyMemberCount());
        }
    }
}
