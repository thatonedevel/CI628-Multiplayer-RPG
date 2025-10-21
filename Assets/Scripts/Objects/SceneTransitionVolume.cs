using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class SceneTransitionVolume : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int enteredPlayers = 0;

    [SerializeField] private string targetScene = "";
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // check we're server / host
        if (IsServer)
        {
            if (other.CompareTag("Player"))
            {
                enteredPlayers++;
                // check that all players are in the volume
                if (enteredPlayers == 1) // TODO: replace this with the party member count once party system is implemented
                {
                    // use network singleton to load the target scene
                    NetworkManager.Singleton.SceneManager.LoadScene(targetScene, UnityEngine.SceneManagement.LoadSceneMode.Single);

                    // TODO: once party system is implemented, move player characters to target positions
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsServer) 
        {
            if (other.CompareTag("Player"))
            {
                enteredPlayers--;
            }
        }
    }
}
