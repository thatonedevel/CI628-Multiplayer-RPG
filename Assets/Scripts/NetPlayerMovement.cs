using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetPlayerMovement : NetworkBehaviour
{
    // input action variables
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;


    private bool isJumping = false;

    // movement speed
    [SerializeField] private float moveSpeed = 5f;

    [Header("Component References")]
    private Rigidbody playerRigidbody;
    private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnNetworkSpawn()
    {
        // find the action mappings
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        interactAction = InputSystem.actions.FindAction("Interact");

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // add a listener for 
    }

    // Update is called once per frame
    void Update()
    {
        // read values from move / jump input actions

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        isJumping = jumpAction.triggered;

        MovePlayerCharacterRPC(moveInput, isJumping);
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

    [Rpc(SendTo.ClientsAndHost)]
    public void ToggleOverworldCameraRPC()
    {
        playerCamera.enabled = !playerCamera.enabled;
    }
}
