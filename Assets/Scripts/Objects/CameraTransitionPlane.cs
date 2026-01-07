using UnityEngine;

public class CameraTransitionPlane : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        // check the normal
        Debug.Log("Left trigger");
    }
}
