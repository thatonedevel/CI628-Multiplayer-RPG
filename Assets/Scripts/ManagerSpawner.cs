using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ManagerSpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> managerNetPrefabs = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrySpawningManagers()
    {
        // call this from the host machine

        if (!NetworkManager.didStart)
        {
            // run this if called before net manager is started
            Debug.LogError("Called TrySpawningManagers before the NetworkManager was started");
        }

        if (!IsServer)
        {
            Debug.LogError("Manager Objects cannot be spawned by a non-host machine");
            return;
        }

        GameObject testReference = null;

        // go through each manager in the prefab list, and check if it already exists
        for (int pfIndex = 0; pfIndex < managerNetPrefabs.Count; pfIndex++)
        {
            // grab tag, check if object with that tag exists
            testReference = GameObject.FindWithTag(managerNetPrefabs[pfIndex].tag);

            if (testReference is null)
            {
                // object does not exist, we can spawn the prefab
                // instantiate the local object
                Debug.Log("CLIENT: Spawning local instance of " + managerNetPrefabs[pfIndex].name);
                GameObject clientObj = Instantiate(testReference);

                // spawn on the network
                clientObj.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
