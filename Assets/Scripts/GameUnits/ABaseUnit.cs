using System;
using UnityEngine;
using Unity.Netcode;

public abstract class ABaseUnit : NetworkBehaviour
{

    // unit stats
    [Header("Unit Stats")]
    public NetworkVariable<int> maxHP = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(0);
    public NetworkVariable<int> defense = new NetworkVariable<int>(0);
    public NetworkVariable<int> attack = new NetworkVariable<int>(0);

    [Header("Inherited References")]
    [SerializeField] protected DamageNumbers damageNumbers;

    // turn taken event
    public static event Action<ulong> TurnTakenEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public virtual void TakeDamage(int damageAmount)
    {
        // take the damage
        int totalDmg = damageAmount - defense.Value;
        currentHP.Value -= totalDmg;

        if (damageNumbers is not null)
        {
            damageNumbers.DisplayDamage(totalDmg);
        }
    }

    protected void InvokeTurnEnd(ulong unitID)
    {
        TurnTakenEvent?.Invoke(unitID);
    }
}
