using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldItem : NetworkBehaviour
{
    [SerializeField] private string Item_ID;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // despawn world self on network
                NetworkObject.Despawn(true);
            }
        }
    }
}
