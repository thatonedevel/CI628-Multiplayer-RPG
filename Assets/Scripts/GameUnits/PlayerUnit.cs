using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerUnit : ABaseUnit
{
    // player events
    public static event Action<ulong> PlayerDamageEvent;
    public static event Action<ulong> PlayerDeathEvent;
    public static event Action<PlayerState, ulong> PlayerStateChangedEvent;

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

        PlayerDamageEvent?.Invoke(GetComponent<NetworkBehaviour>().NetworkObjectId);

        // check if hp <= 0
        if (currentHP <= 0)
        {
            currentHP = 0;
            PlayerDeathEvent?.Invoke(GetComponent<NetworkBehaviour>().NetworkObjectId);
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

    [Rpc(SendTo.Server)]
    public void UpdatePlayerStateRPC(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.MOVING:
                break;
            case PlayerState.BATTLE:
                break;
            case PlayerState.SPECTATOR:
                break;
            case PlayerState.VIEWING_INVENTORY:
                break;
            case PlayerState.IDLE:
                break;
            default:
                break;
        }

        currentPlayerState = newState;
        // raise player state changed event
        PlayerStateChangedEvent?.Invoke(newState, GetComponent<NetworkObject>().NetworkObjectId);
    }
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