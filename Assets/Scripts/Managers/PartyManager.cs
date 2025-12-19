using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PartyManager : NetworkBehaviour
{
    // class for managing the rpg party & game session

    private List<GameObject> playerGameObjects = new();
    private List<string> partyMemberGUIDs = new();
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
            if (IsServer)
            {
                Singleton = this;
                // subscribe to client connect / disconnect events
                NetworkManager.OnClientConnectedCallback += ClientConnectListener;
                NetworkManager.OnConnectionEvent += ConnectionEventListener;

                // add the host's character to the party
                playerGameObjects.Add(GameObject.FindWithTag("Player"));
                Debug.Log("SERVER: Added host character");
            }
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

        Debug.Log("SERVER: Added player ID: " + id + " to party");
    }

    public int GetAlivePartyMemberCount()
    {
        int count = 0;
        for (int i = 0; i < playerGameObjects.Count; i++)
        {
            int hp = playerGameObjects[i].GetComponent<ABaseUnit>().currentHP.Value;

            if (hp > 0)
                count++;
        }

        return count;
    }

    public int GetTotalPlayerCount()
    {
        return playerGameObjects.Count;
    }

    public PlayerUnit GetPartyMember(int index)
    {
        if (index < 0 || index >= playerGameObjects.Count)
            return null;

        return playerGameObjects[index].GetComponent<PlayerUnit>();
    }

    public List<int> GetAlivePlayerIndices()
    {
        List<int> aliveIndices = new List<int>();

        for (int i = 0; i < playerGameObjects.Count; i++)
        {
            var unit = playerGameObjects[i].GetComponent<ABaseUnit>();

            if (unit.currentHP.Value > 0)
                aliveIndices.Add(i);
        }

        return aliveIndices;
    }

    public void UpdateStateForAllPlayers(PlayerState newState)
    {
        if (!IsServer)
        {
            Debug.LogError("Method must be called from server");
            return;
        }

        for (int i = 0; i < playerGameObjects.Count; i++)
        {
            playerGameObjects[i].GetComponent<PlayerUnit>().UpdatePlayerStateRPC(newState);
        }
    }

    private void ClientConnectListener(ulong clientID)
    {
        if (!IsServer)
        {
            return;
        }

        Debug.Log("SERVER: Client with ID: " + clientID + " joined");
        // get the character owned by the player
        GameObject[] pcObjects = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < pcObjects.Length; i++) 
        {
            if (pcObjects[i].GetComponent<NetworkObject>().OwnerClientId == clientID)
            {
                // check if the party is full
                if (playerGameObjects.Count < 4)
                {
                    // add player
                    Debug.Log("SERVER: Added player to party");
                    playerGameObjects.Add(pcObjects[i]);
                    //partyMemberGUIDs.Add(pcObjects[i].GetComponent<PlayerUnit>().playerGUID.Value);
                }
            }
        }
    }

    private void ConnectionEventListener(NetworkManager manRef, ConnectionEventData dat)
    {
        if (dat.EventType == ConnectionEvent.ClientConnected)
        {
            Debug.Log("SERVER: client detected");
        }
    }
}
