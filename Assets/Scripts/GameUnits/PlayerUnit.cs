using System;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerUnit : ABaseUnit
{
    // player events
    public static event Action<ulong, int> OnPlayerDamaged;
    public static event Action<ulong> OnPlayerKilled;
    public static event Action<PlayerState, ulong> PlayerStateChangedEvent;

    public const string OBJECT_ID_KEY = "current_owned_pc_id";

    [Header("Player Attributes")]
    private PlayerState currentPlayerState = PlayerState.IDLE;
    [SerializeField] public string playerName = "Player";

    private bool isBotPlayer = false;

    InputAction confirmAction;
    InputAction backAction;

    //public NetworkVariable<string> playerGUID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // input stuff

        // set up guid
        if (IsClient || IsHost)
        {
            //playerGUID.Value = ClientData.clientGUID;
            // set the id of the current owned player object
            PlayerPrefs.SetInt(OBJECT_ID_KEY, (int)NetworkObjectId);
        }
    }

    private void Update()
    {
        switch (currentPlayerState)
        {
            case PlayerState.BATTLE:
                break;
        }
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

        if (currentHP.Value <= 0)
        {
            currentHP.Value = 0;
            OnPlayerKilled?.Invoke(NetworkObjectId);
        }
        else
        {
            OnPlayerDamaged?.Invoke(NetworkObjectId, damageAmount - defense.Value);
            DamageTakenRPC(damageAmount - defense.Value);
        }
    }

    // client side RPC for when player is damaged
    [Rpc(SendTo.ClientsAndHost)]
    public void DamageTakenRPC(int amount)
    {
        // play damage animaton/sound here
        // also update UI
        Debug.Log("CLIENT: Taken damage: " + amount);
        // call dmg numbers display
        damageNumbers.DisplayDamage(amount);
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
        // check if the player was spectating and now is not
        if (currentPlayerState == PlayerState.SPECTATOR && newState != PlayerState.SPECTATOR)
        {
            // call spectate exit
            EndSpectateClientRPC();
        }

        switch (newState)
        {
            case PlayerState.MOVING:
                GetComponent<NetPlayerMovement>().enabled = true;
                break;
            case PlayerState.BATTLE:
                GetComponent<NetPlayerMovement>().enabled = false;

                break;
            case PlayerState.SPECTATOR:
                BeginSpectateClientRPC();
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

    // battle methods
    [Rpc(SendTo.Server)]
    public void AttackEnemyRPC(int enemyIndex)
    {
        // send message to battle manager
    }

    public void UseItem()
    {

    }

    public void UseSkill()
    {

    }

    // specatator entry method
    [Rpc(SendTo.ClientsAndHost)]
    private void BeginSpectateClientRPC()
    {
        // disable the following: renderer, collision, movement and cam
        GetComponent<Camera>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<NetPlayerMovement>().enabled = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndSpectateClientRPC()
    {
        GetComponent<Camera>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<NetPlayerMovement>().enabled = true;
    }


    // util methods
    [Rpc(SendTo.ClientsAndHost)]
    private void StorePCObjectIdRPC(ulong id)
    {
        PlayerPrefs.SetInt(OBJECT_ID_KEY, (int)id);
    }
}
// player states

public enum PlayerState
{
    MOVING,
    BATTLE,
    SPECTATOR,
    VIEWING_INVENTORY, // covers all "viewing ui" cases
    IDLE
}