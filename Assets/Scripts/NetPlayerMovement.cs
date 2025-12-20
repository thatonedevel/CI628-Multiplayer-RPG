using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetPlayerMovement : NetworkBehaviour
{
    // input action variables
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private NetworkVariable<bool> canMove = new NetworkVariable<bool>(true);

    private bool isJumping = false;

    // movement speed
    [SerializeField] private float moveSpeed = 5f;

    [Header("Component References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener playerAudioListner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the action mappings
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        interactAction = InputSystem.actions.FindAction("Interact");

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // check if we're the machine that owns this player
        playerCamera.enabled = IsOwner;
        playerAudioListner.enabled = IsOwner;
    }

    // Update is called once per frame
    void Update()
    {
        // check if this machine owns this player character
        if (IsOwner)
        {
            // read values from move / jump input actions

            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            isJumping = jumpAction.triggered;

            if (canMove.Value)
                MovePlayerCharacterRPC(moveInput, isJumping);
        }
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerCharacterRPC(Vector2 movement, bool jumped)
    {
        // calculate new player position (not accounting y axis)
        Vector3 newPosition = transform.position + new Vector3(movement.x, 0, movement.y) * moveSpeed * Time.deltaTime;
        
        // update rb position
        playerRigidbody.MovePosition(newPosition);

        // if we can jump and jump was pressed, add an impulse force
        if (jumped && CanCharacterJump())
        {
            playerRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    private bool CanCharacterJump()
    {
        // send raycast down - if it hits we can jump
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1.1f);

        return hitInfo.collider is not null;
    }

    // camera methods

    [Rpc(SendTo.ClientsAndHost)]
    public void ToggleOverworldCameraRPC()
    {
        Debug.Log("CLIENT: Camera toggled");
        // check we own this object
        if (IsOwner)
        {
            playerCamera.enabled = !playerCamera.enabled;
            playerAudioListner.enabled = !playerAudioListner.enabled;
        }  
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DisableOverworldCameraRPC()
    {
        playerCamera.enabled = false;
        playerAudioListner.enabled = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EnableOverworldCameraRPC()
    {
        playerCamera.enabled = false;
        playerAudioListner.enabled = false;
    }

    public void OnCharStateTransition(PlayerState newState, ulong playerId)
    {
        // only run this method client side
        if (!IsClient)
            return;

        // check if the changed state is for this char & check we also own this character

        if (playerId == GetComponent<NetworkBehaviour>().NetworkObjectId && IsOwner)
        {
            // depending on new state, enable / disable movement
            switch (newState)
            {
                case PlayerState.MOVING:
                    canMove.Value = true;
                    break;
                case PlayerState.SPECTATOR:
                    canMove.Value = true;
                    break;
                default:
                    canMove.Value = false;
                    break;
            }
        }
    }
}
