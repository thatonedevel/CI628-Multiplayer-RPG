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

    private void OnClientConnected()
    {
        // add the new player object to the party
    }
}
