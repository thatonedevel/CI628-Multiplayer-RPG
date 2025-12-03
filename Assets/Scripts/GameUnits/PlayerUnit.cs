using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerUnit : ABaseUnit
{
    // player events
    public static event Action<ulong> PlayerDamageEvent;
    public static event Action<ulong> PlayerDeathEvent;
    public static event Action<PlayerState, ulong> PlayerStateChangedEvent;

    [Header("Player Attributes")]
    private PlayerState currentPlayerState = PlayerState.IDLE;
    [SerializeField] private string playerName = "Player";

    private bool isBotPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public new void OnNetworkSpawn()
    {
        // add self to host party manager
        Debug.Log("CLIENT: Joined Party");
        PartyManager.Singleton.AddPartyMemberRPC(NetworkObjectId);
    }

    // Update is called once per frame
    public override void TakeDamage(int damageAmount)
    {
        // check we are the server
        if (!(IsServer || IsHost))
            return;
        // reduce damage by amount minus def
        currentHP.Value -= (damageAmount - defense.Value);

        PlayerDamageEvent?.Invoke(GetComponent<NetworkBehaviour>().NetworkObjectId);

        // check if hp <= 0
        if (currentHP.Value <= 0)
        {
            currentHP.Value = 0;
            PlayerDeathEvent?.Invoke(GetComponent<NetworkBehaviour>().NetworkObjectId);
        }
    }

    // client side RPC for when player is damaged
    [Rpc(SendTo.ClientsAndHost)]
    public void DamageTakenRPC(ulong damageSource)
    {
        // play damage animaton/sound here
        // also update UI
        Debug.Log("Attacked by enemy with ID");
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
                GetComponent<NetPlayerMovement>().enabled = true;
                break;
            case PlayerState.BATTLE:
                GetComponent<NetPlayerMovement>().enabled = false;

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


    [Rpc(SendTo.ClientsAndHost)]
    public void SetDisplayToOverworldRPC()
    {
        // enable player cam
        GetComponent<Camera>().enabled = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetDisplayToBattleRPC()
    {
        GetComponent<Camera>().enabled = false;
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