using System;
using UnityEngine;
using Unity.Netcode;

public abstract class ABaseUnit : NetworkBehaviour
{

    // unit stats
    public NetworkVariable<int> maxHP = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(0);
    public NetworkVariable<int> defense = new NetworkVariable<int>(0);
    public NetworkVariable<int> attack = new NetworkVariable<int>(0);

    // turn taken event
    public static event Action<ulong> TurnTakenEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public abstract void TakeDamage(int damageAmount);
}
