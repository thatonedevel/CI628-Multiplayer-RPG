using UnityEngine;
using Unity.Netcode;

public abstract class ABaseUnit : NetworkBehaviour
{

    // unit stats
    public int maxHP = 0;
    public int currentHP = 0;
    public int defense = 0;
    public int attack = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public abstract void TakeDamage(int damageAmount);
}
