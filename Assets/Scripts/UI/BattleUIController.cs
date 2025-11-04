using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BattleUIController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // button reference variables
    private Button fightButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button fleeButton;

    private ListView optionsListView;

    private List<BasicEnemy> basicEnemyData = new List<BasicEnemy>();

    void Start()
    {
        // grab the UI document
        UIDocument uiDocument = GetComponent<UIDocument>();

        // get references to all the buttons

        fightButton = uiDocument.rootVisualElement.Query<Button>("FightButton");
        skillsButton = uiDocument.rootVisualElement.Query<Button>("SkillButton");
        itemsButton = uiDocument.rootVisualElement.Query<Button>("ItemButton");
        fleeButton = uiDocument.rootVisualElement.Query<Button>("FleeButton");

        optionsListView = uiDocument.rootVisualElement.Query<ListView>("SelectionListView");

        // event subscription
        fightButton.clicked += OnFightPressed;
        skillsButton.clicked += OnSkillsPressed;
        itemsButton.clicked += OnItemsPressed;
        fleeButton.clicked += OnFleePressed;
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

    // event functions for when each button is pressed

    public void OnFightPressed()
    {
        // get a list of all enemies, add them to the selection listview
    }

    public void OnSkillsPressed()
    {

    }

    public void OnItemsPressed()
    {

    }

    public void OnFleePressed()
    {

    }

    // rpcs for requesting & responding with enemy list data

    [Rpc(SendTo.Server)]
    private void GetEnemyListRPC()
    {
        // use battle manager singelton to get a list of enemies

    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ReturnEnemyList(List<BasicEnemy> targetList, RpcParams rpcParams = default)
    {

    }
}
