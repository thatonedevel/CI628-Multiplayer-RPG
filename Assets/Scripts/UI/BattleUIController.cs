using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class BattleUIController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // button reference variables
    private Button fightButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button fleeButton;

    private ListView enemySelectionListView;

    // panel listviews are displayed over
    // TODO: add slide-out anim
    private VisualElement mainSelectionPanel;

    private List<BasicEnemy> basicEnemyData = new List<BasicEnemy>();

    [Header("Scriptable Objects for data sources")]
    [SerializeField] private EnemyDataSourceSO enemyData;

    // lamdbda for making list items

    Func<VisualElement> makeItem = () => new Label();

    void Start()
    {
        // grab the UI document
        UIDocument uiDocument = GetComponent<UIDocument>();

        // get references to all the buttons

        fightButton = uiDocument.rootVisualElement.Query<Button>("FightButton");
        skillsButton = uiDocument.rootVisualElement.Query<Button>("SkillButton");
        itemsButton = uiDocument.rootVisualElement.Query<Button>("ItemButton");
        fleeButton = uiDocument.rootVisualElement.Query<Button>("FleeButton");

        enemySelectionListView = uiDocument.rootVisualElement.Query<ListView>("EnemySelectionListView");
        mainSelectionPanel = uiDocument.rootVisualElement.Query<VisualElement>("OptionsPanel");

        // event subscription
        fightButton.clicked += OnFightPressed;
        skillsButton.clicked += OnSkillsPressed;
        itemsButton.clicked += OnItemsPressed;
        fleeButton.clicked += OnFleePressed;

        // add the make item function to list view
        enemySelectionListView.makeItem = makeItem;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // rpc to disable buttons when not player's turn
    [Rpc(SendTo.ClientsAndHost)]
    public void ToggleActionButtonsRPC()
    {
        fightButton.SetEnabled(!fightButton.enabledSelf);
        skillsButton.SetEnabled(!skillsButton.enabledSelf);
        itemsButton.SetEnabled(!itemsButton.enabledSelf);
        fleeButton.SetEnabled(!fleeButton.enabledSelf);
    }

    // event functions for when each button is pressed

    public void OnFightPressed()
    {
        // clear out previous enemy data
        enemyData.ClearEnemies();
        // get a list of all enemies, add them to the selection listview
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            BasicEnemy dat = enemy.GetComponent<BasicEnemy>();

            enemyData.AddEnemyToList(dat);
        }

        // make display view visible
        mainSelectionPanel.style.visibility = Visibility.Visible;
        enemySelectionListView.style.visibility = Visibility.Visible;
    }

    public void OnSkillsPressed()
    {
        // TODO: implement when skills are added
    }

    public void OnItemsPressed()
    {

    }

    public void OnFleePressed()
    {

    }

    // rpcs for requesting & responding with enemy list data

    [Rpc(SendTo.Server)]
    private void GetEnemyListRPC(RpcParams rpcParams = default)
    {
        // use battle manager singelton to get a list of enemies
        ulong[] enemyIdArr = BattleManager.Singleton.GetAllEnemyUnits();
        ReturnEnemyListRPC(enemyIdArr, RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ReturnEnemyListRPC(ulong[] targetList, RpcParams rpcParams = default)
    {
        // we have the enemy list yippee
        // get list of enemy objects on the client side
        for (int i = 0; i < targetList.Length; i++)
        {
            NetworkObject localEnemy = GetNetworkObject(targetList[i]);

            basicEnemyData.Add(localEnemy.GetComponent<BasicEnemy>());
        }
    }
}

public enum MenuType
{
    ENEMIES,
    SKILLS,
    ITEMS
}