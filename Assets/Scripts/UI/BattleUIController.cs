using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class BattleUIController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // button reference variables
    private Button fightButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button fleeButton;

    void Start()
    {
        // grab the UI document
        UIDocument uiDocument = GetComponent<UIDocument>();

        // get references to all the buttons

        fightButton = uiDocument.rootVisualElement.Q<Button>("FightButton");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // rpc to disable buttons when not player's turn
    [Rpc(SendTo.ClientsAndHost)]
    public void ToggleActionButtonsRPC()
    {

    }
}
