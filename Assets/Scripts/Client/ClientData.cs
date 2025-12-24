using UnityEngine;
using System;

public class ClientData : MonoBehaviour
{
    private const string GUID_KEY = "client_guid";

    public static string clientGUID { get; private set; }

    // used to store client side data such as client GUID

    private void Start()
    {
        // check if we have a guid
        if (!PlayerPrefs.HasKey(GUID_KEY))
        {
            // generate a guid for the client and store it
            string generatedGuid = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GUID_KEY, generatedGuid);
        }

        // store the guid in a variable for easy access
        clientGUID = PlayerPrefs.GetString(GUID_KEY);
    }
}
