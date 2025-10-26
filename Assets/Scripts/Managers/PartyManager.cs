using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PartyManager : NetworkBehaviour
{
    private List<GameObject> playerGameObjects = new List<GameObject>();
    public static PartyManager Singleton;

    // events
    public event Action<int> PlayerStateChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Rpc(SendTo.Server)]
    public void AddPartyMemberRPC(ulong id) // use a ulong as we need to use a serializable type
    {
        // since we have the network object, use the id to find the server side verion
        List<GameObject> serverPlayers = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Player", serverPlayers);

        // linear search for the matching id
        for (int i = 0; i < serverPlayers.Count; i++)
        {
            if (serverPlayers[i].GetComponent<NetworkObject>().NetworkObjectId == id)
            {
                playerGameObjects.Add(serverPlayers[i]);
                break;
            }
        }
    }
}
