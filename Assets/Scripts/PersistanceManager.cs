using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PersistanceManager : MonoBehaviour
{
    // network manager reference
    [SerializeField] private List<GameObject> persistentObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // called when host or client starts
        Debug.Log("Running");
        for (int i = 0; i < persistentObjects.Count; i++)
        {
            DontDestroyOnLoad(persistentObjects[i]);
        }

        // we're good to move to the title screen
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }
}
