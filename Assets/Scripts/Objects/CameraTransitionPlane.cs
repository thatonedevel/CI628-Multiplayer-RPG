using UnityEngine;

public class CameraTransitionPlane : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered trigger");

        if (collision.tag == "Player")
        {
            bool success = CheckObjectIsEnteringFromFront(collision.gameObject);
            Debug.Log("Was object entering from the front? " + success);
        }
    }

    private bool CheckObjectIsEnteringFromFront(GameObject target)
    {
        // calculate current distance between target and self
        Vector3 rel = target.transform.position - transform.position;
        Vector3 forwardVec = transform.forward;

        if (rel.normalized == forwardVec)
            return true;

        return false;
    }
}
