using UnityEngine;
using Unity.Netcode;

public class Zombie : BasicEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void AttackPlayer()
    {
        // base isn't implemented

        if (IsServer)
        {
            // pick a random player to attack
        }
    }
}
