using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] private List<GameObject> persistentObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < persistentObjects.Count; i++)
        {
            DontDestroyOnLoad(persistentObjects[i]);
        }
    }
}
