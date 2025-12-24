using Unity.Netcode;
using UnityEngine;

public class EnemyOverworldBehaviour : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer)
            return;

        // make sure we only run this on the server side
        if (collision.gameObject.CompareTag("Player"))
            HitPlayerCharRPC();
    }

    [Rpc(SendTo.Server)]
    public void HitPlayerCharRPC()
    {
        // stop the movement state
        // call the battle transition function
        GameController.Singleton.TransitionToBattle();
    }
}

public enum OverworldStates
{
    IDLE,
    WANDERING,
    CHASING,
    PATROL
}