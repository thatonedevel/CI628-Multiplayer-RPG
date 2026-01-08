using UnityEngine;

public class PlayerCharAnimationControl : MonoBehaviour
{
    [SerializeField] private Rigidbody characterRigidbody;
    [SerializeField] private Animator characterAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterAnimator.SetBool("IsMoving", characterRigidbody.linearVelocity.magnitude > 0);
    }
}
