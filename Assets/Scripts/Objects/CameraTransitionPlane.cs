using Unity.Netcode;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;

public class CameraTransitionPlane : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("New Cam Settings: Update flags")]
    [SerializeField] private bool changeCameraPosition;
    [SerializeField] private bool changeCameraRotation;
    [SerializeField] private bool changeFieldOfView;

    [Header("New Cam Settings - Values")]
    [SerializeField] private Vector3 relativeCameraPosition;
    [SerializeField] private Vector3 relativeEulerAngles;
    [SerializeField] private float fieldOfView;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered trigger");

        if (collision.tag == "Player")
        {
            bool success = CheckObjectIsEnteringFromFront(collision.gameObject);
            if (success) 
            {
                // check this is the local player
                if (collision.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    // get reference to the camera
                    Camera playerCam = collision.gameObject.GetComponentInChildren<Camera>();

                    // adjust settings as needed
                    if (changeCameraPosition) playerCam.transform.localPosition = relativeCameraPosition;
                    if (changeCameraRotation) playerCam.transform.localEulerAngles = relativeEulerAngles;
                    if (changeFieldOfView) playerCam.fieldOfView = fieldOfView;
                }
            }
        }
    }

    private bool CheckObjectIsEnteringFromFront(GameObject target)
    {
        // (cannon, 2000, a:8/1/25) https://discussions.unity.com/t/one-way-trigger/400023/2
        // make sure we use a relative vector as described by the docs
        Vector3 rel = transform.position - target.transform.position;
        float angle = Vector3.Angle(rel, transform.forward);

        return angle > 90;
    }
}
