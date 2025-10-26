using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerUnit : ABaseUnit
{
    // player events
    public event Action PlayerDamageEvent;
    public event Action PlayerDeathEvent;

    private PlayerState currentPlayerState = PlayerState.IDLE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void TakeDamage(int damageAmount)
    {
        // reduce damage by amount minus def
        currentHP -= (damageAmount - defense);

        PlayerDamageEvent?.Invoke();

        // check if hp <= 0
        if (currentHP <= 0)
        {
            currentHP = 0;
            PlayerDeathEvent?.Invoke();
        }
    }

    public void OnNetworkInstantiate()
    {
        if (IsOwner)
        {
            // called when spawned on network
            // add self to party
            PartyManager.Singleton.AddPartyMemberRPC(GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    public PlayerState GetPlayerState()
    {
        return currentPlayerState;
    }

    // used for updating player state

}


// player states

public enum PlayerState
{
    MOVING,
    BATTLE,
    SPECTATOR,
    VIEWING_INVENTORY, // covers all "viewing ui" cases
    IDLE,
}