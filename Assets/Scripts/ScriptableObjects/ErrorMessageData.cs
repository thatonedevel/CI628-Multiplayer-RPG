using UnityEngine;

[CreateAssetMenu(fileName = "ErrorMessageData", menuName = "Scriptable Objects/ErrorMessageData")]
public class ErrorMessageData : ScriptableObject
{
    public string errorMessage;

    public void SetError(string rawMessage)
    {
        // sets the message using a given format
        errorMessage = "Error: " + rawMessage;
    }
}
