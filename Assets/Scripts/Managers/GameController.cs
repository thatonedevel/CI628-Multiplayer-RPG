using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameController : NetworkBehaviour
{
    // purely server / host side game controller class

    // reference to party - state is on a per-player basis

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameController Singleton;
    public NetworkVariable<bool> inBattle = new NetworkVariable<bool>(false);

    // scene transition info
    private string overworldSceneName = "";
    private List<Transform> playerOverworldLocations = new();

    private void Start()
    {
        // register to the network start events
        NetworkManager.OnServerStarted += OnNetworkStarted;
    }

    private void OnNetworkStarted()
    {
        if (IsServer || IsHost)
        {
            // create singleton
            if (Singleton == null)
            {
                Debug.Log("SERVER: Started Game Manager");
                Singleton = this;

                // add scene transition detection event callback
                NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TransitionToBattle()
    {
        if (!IsServer)
            return;

        Debug.Log("SERVER: Transitioning to battle state");

        // grab all player objects
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playerObjs.Length; i++) 
        {
            playerObjs[i].GetComponent<PlayerUnit>().UpdatePlayerStateRPC(PlayerState.BATTLE);
        }

        // transition to battle scene
        NetworkManager.Singleton.SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
    }

    public void TransitionToOverworld()
    {

    }

    // function that runs on a player state change
    private void OnAnyPlayerStateChange(PlayerState newState)
    {
        switch(newState)
        {
            case PlayerState.BATTLE:
                inBattle.Value = true;
                break;
            case PlayerState.MOVING:
                inBattle.Value = false;
                break;
        }
    }

    private void SceneManager_OnSceneEvent(SceneEvent eventData)
    {
        switch (eventData.SceneEventType)
        {
            case SceneEventType.ActiveSceneChanged:
                OnGlobalSceneChange();
                break;
        }
    }

    private void OnGlobalSceneChange()
    {
        if (!IsServer)
            return;

        Debug.Log("SERVER: Synchronised scene transition detected");
        if (NetworkManager.SceneManager.GetSynchronizedScenes()[0].name == "BattleScene")
        {
            Debug.Log("SERVER: Entered Battle Scene");
        }
        else
        {
            // overworld
            // call camera enable on each player
            // move all players to original position
            for (int i = 0; i < PartyManager.Singleton.GetAlivePartyMemberCount(); i++)
            {
                if (overworldSceneName != "")
                {
                    var memberTrans = PartyManager.Singleton.GetPartyMember(i).GetComponent<Transform>();
                    memberTrans.position = playerOverworldLocations[i].position;
                    memberTrans.rotation = playerOverworldLocations[i].rotation;
                }
            }
        }
    }
}
