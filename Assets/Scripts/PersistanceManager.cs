using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PersistanceManager : NetworkBehaviour
{
    // network manager reference
    [SerializeField] private List<GameObject> persistentObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        NetworkManager.OnClientStarted += OnClientStart;
    }

    void OnClientStart()
    {
        // called when host or client starts
        Debug.Log("Running");
        for (int i = 0; i < persistentObjects.Count; i++)
        {
            DontDestroyOnLoad(persistentObjects[i]);
        }
        
        // we're good to move to the test scene
        NetworkManager.SceneManager.LoadScene("TestDungeon_Room0", LoadSceneMode.Single);
        // load battle scene in additive mode (we'll switch active scenes when changing between battle / overworld)
        NetworkManager.SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
    }
}
