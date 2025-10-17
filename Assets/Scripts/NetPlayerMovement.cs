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
    }

    // Update is called once per frame
    void Update()
    {
        // read values from move / jump input actions

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        MovePlayerCharacterRPC(moveInput, isJumping);
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerCharacterRPC(Vector2 movement, bool jumped)
    {

    }
}
