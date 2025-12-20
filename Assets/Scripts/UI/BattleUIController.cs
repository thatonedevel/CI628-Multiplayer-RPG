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

    [SerializeField] private BattleManager battleManagerLocalRef;

    // lamdbda for making list items

    Func<VisualElement> makeItem = () => new Label();

    Func<VisualElement> makeButton = () =>
    {
        Button listButton = new Button();
        

        return listButton;
    };

    [Rpc(SendTo.ClientsAndHost)]
    public void ActivationRPC(ulong startingPlayerID)
    {
        Debug.Log("CLIENT: Setting up battle UI");
        // grab the UI document
        UIDocument uiDocument = GetComponent<UIDocument>();

        // get references to all the buttons

        fightButton = uiDocument.rootVisualElement.Query<Button>("FightButton");
        skillsButton = uiDocument.rootVisualElement.Query<Button>("SkillButton");
        itemsButton = uiDocument.rootVisualElement.Query<Button>("ItemButton");
        fleeButton = uiDocument.rootVisualElement.Query<Button>("FleeButton");

        enemySelectionListView = uiDocument.rootVisualElement.Query<ListView>("SelectionListView");
        mainSelectionPanel = uiDocument.rootVisualElement.Query<VisualElement>("OptionsPanel");

        // event subscription
        fightButton.clicked += OnFightPressed;
        skillsButton.clicked += OnSkillsPressed;
        itemsButton.clicked += OnItemsPressed;
        fleeButton.clicked += OnFleePressed;


        if (enemySelectionListView is null)
            Debug.Log("lv is null");

        // add the make item function to list view
        enemySelectionListView.makeItem = makeItem;

        // if we are the server/host, subscribe to these events
        if (IsHost)
        {
            BattleManager.OnCompletedTurnEndProcessing += TurnEndedHandler;
        }

        // use this to check if we need to enable / disable the buttons
        if ((ulong)PlayerPrefs.GetInt(PlayerUnit.OBJECT_ID_KEY) == startingPlayerID)
        {
            // we own the player, enable the ui
            Debug.Log("CLIENT: Enabling UI interaction");
            EnableBattleUI();
        }
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
        Debug.Log("CLIENT: Fight Pressed");
        // clear out previous enemy data
        enemyData.ClearEnemies();
        // get a list of all enemies, add them to the selection listview
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            BasicEnemy dat = enemy.GetComponent<BasicEnemy>();

            enemyData.AddEnemyToList(dat);
        }

        // refresh
        enemySelectionListView.RefreshItems();

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

    [Rpc(SendTo.Server)]
    private void TargetEnemyRPC(int enemyIndex)
    {
        BattleManager.Singleton.EnemyAttacked(enemyIndex);
    }

    // client side rpc to enable ui control
    [Rpc(SendTo.ClientsAndHost)]
    public void EnableBattleUIRPC(ulong currentPlayerUnit, bool isPlayerTurn = true)
    {
        if (isPlayerTurn)
        {
            int indexToCheck = -1;
            // get the player unit that is owned by this machine
            GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < playerUnits.Length; i++)
            {
                if (playerUnits[i].GetComponent<NetworkObject>().NetworkObjectId == currentPlayerUnit)
                {
                    indexToCheck = i;
                    break;
                }
            }

            if (indexToCheck == -1)
                return;

            // we have the player, check if this machine owns it
            if (playerUnits[indexToCheck].GetComponent<NetworkObject>().IsOwner)
            {
                Debug.Log("CLIENT: Enabling battle UI");
                // we do, enable ui on this machine
                EnableBattleUI();
            }
            else
            {
                Debug.Log("CLIENT: Disabling battle UI");
                // different player turn
                DisableBattleUI();
            }
        }
        else
        {
            DisableBattleUI();
        }
    }

    // event handlers
    private void TurnEndedHandler(int unitIndex, ulong netID, bool isPlayerUnit)
    {
        if (IsServer)
            EnableBattleUIRPC(netID, isPlayerUnit);
    }

    private void EnableBattleUI()
    {
        // enable buttons
        fightButton.enabledSelf = true;
        fleeButton.enabledSelf = true;
        itemsButton.enabledSelf = true;
        skillsButton.enabledSelf = true;
    }

    private void DisableBattleUI()
    {
        // disable buttons
        fightButton.enabledSelf = false;
        fleeButton.enabledSelf = false;
        itemsButton.enabledSelf = false;
        skillsButton.enabledSelf = false;
    }
}

public enum MenuType
{
    ENEMIES,
    SKILLS,
    ITEMS
}