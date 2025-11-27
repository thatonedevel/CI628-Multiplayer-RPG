using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    void Start()
    {
        if (IsServer)
        {
            // create singleton
            if (Singleton == null)
            {
                Singleton = this;

                // add scene transition detection event callback
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

        Debug.Log("Transitioning to battle state");

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

    private void OnGlobalSceneChange(Scene destScene, LoadSceneMode mode)
    {
        if (!IsServer)
            return;

        if (destScene.name == "Battle")
        {
            Debug.Log("Entered Battle Scene");
            // call the battle manager start function
            BattleManager.Singleton.BattleStartedRPC();
        }
    }
}
