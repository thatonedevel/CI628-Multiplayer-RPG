using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    // network manager reference
    [SerializeField] private NetworkManager netManager;
    [SerializeField] private List<GameObject> persistentObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        netManager.OnClientStarted += OnClientStart;
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
        netManager.SceneManager.LoadScene("TestDungeon_Room0", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
