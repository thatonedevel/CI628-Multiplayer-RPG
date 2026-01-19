using UnityEngine;

public class PlayerCharAnimationControl : MonoBehaviour
{
    [SerializeField] private NetPlayerMovement movementController;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool wasFlippedOnLastFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterAnimator.SetBool("IsMoving", movementController.isMoving);

        if (wasFlippedOnLastFrame)
        {
            spriteRenderer.flipX = movementController.velX <= 0; // allow for flip preservation when still
        }
        else
        {
            spriteRenderer.flipX = movementController.velX < 0;
        }

        // update flipped on last frame flag
        wasFlippedOnLastFrame = spriteRenderer.flipX;
    }
}
