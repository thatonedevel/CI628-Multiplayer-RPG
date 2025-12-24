using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ReadyCheck : NetworkBehaviour
{
    // reference to string message so
    [SerializeField] private StringDataSO readyMessageObj;
    
    private bool isPlayerReady = false;
    private List<ulong> readyPlayers = new();


    // ui references
    private Label portalOpenLabel;
    private Button readyButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement rootVE = GetComponent<UIDocument>().rootVisualElement;

        portalOpenLabel = rootVE.Query<Label>("PortalMessage");
        readyButton = rootVE.Query<Button>("ReadyButton");

        // subscribe to click event
        readyButton.clicked += OnReadyClicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReadyClicked()
    {
        // invert player ready
        isPlayerReady = !isPlayerReady;

        // update the string message
        if (isPlayerReady)
            readyMessageObj.messageData = "Ready";
        else
            readyMessageObj.messageData = "Not Ready";

        //update on server
        UpdateReadyStatusRpc(NetworkManager.LocalClientId, isPlayerReady);
    }

    [Rpc(SendTo.Server)]
    private void UpdateReadyStatusRpc(ulong playerID, bool newStatus)
    {
        Debug.Log("SERVER: Updating ready status");
        if (newStatus)
        {
            // add
            readyPlayers.Add(playerID);
        }
        else
        {
            readyPlayers.Remove(playerID);
        }

        // check if all players are ready

        if (readyPlayers.Count == PartyManager.Singleton.GetTotalPlayerCount())
        {
            Debug.Log("SERVER: All players are ready");
            // we can start the game
            // update gui state
            UpdateReadyGuiRPC();
            GameController.Singleton.SendPartyToTheWorld();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateReadyGuiRPC()
    {
        // disable button interaction
        readyButton.enabledSelf = false;
        // make portal label visible
        portalOpenLabel.visible = true;
    }
}
