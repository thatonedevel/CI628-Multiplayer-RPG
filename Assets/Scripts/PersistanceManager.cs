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
    void Start()
    {
        for (int i = 0; i < persistentObjects.Count; i++)
        {
            DontDestroyOnLoad(persistentObjects[i]);
        }

        // check that the net manager is started
        if (netManager.didStart)
        {
            // we're good to move to the test scene
            netManager.SceneManager.LoadScene("TestScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
