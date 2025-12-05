using System;
using UnityEngine;
using Unity.Netcode;

public class BasicEnemy : ABaseUnit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // enemy-specific events
    public static event Action<ulong> EnemyDamagedEvent;
    public static event Action<ulong> EnemyKilledEvent;

    public string enemyBaseName = string.Empty;

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
        if (!IsServer)
            return;

        base.TakeDamage(damageAmount);

        // check current hp
        if (currentHP.Value <= 0)
        {
            currentHP.Value = 0;
            // fire unit death event
            EnemyKilledEvent?.Invoke(NetworkObjectId);
        }
        else
        {
            // enemy was damaged but not killed
            EnemyDamagedEvent?.Invoke(NetworkObjectId);
        }
    }

    public virtual void AttackPlayer()
    {
        if (IsServer)
        {
            // enemy attack logic
            // pick a player to attack
            int index = UnityEngine.Random.Range(0, PartyManager.Singleton.GetAlivePartyMemberCount());

            int trueTarget = PartyManager.Singleton.GetAlivePlayerIndices()[index];

            // damage the player
            PartyManager.Singleton.GetPartyMember(trueTarget).TakeDamage(attack.Value);
        }
    }
}
