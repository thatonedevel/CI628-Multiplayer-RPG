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

    // scrolling buttons
    private Button scrollUpButton;
    private Button scrollDownButton;

    

    // panel listviews are displayed over
    // TODO: add slide-out anim
    private VisualElement mainSelectionPanel;
    private VisualElement targetSelectionVBox;
    //private VisualElement scrollButtonPanel;

    // target buttons
    private List<Button> targetButtons = new();

    private List<BasicEnemy> basicEnemyData = new List<BasicEnemy>();

    [Header("Scriptable Objects for data sources")]
    [SerializeField] private EnemyDataSourceSO enemyData;

    [SerializeField] private BattleManager battleManagerLocalRef;

    [SerializeField] private BattleTargetDataSource targetButtonsList;

    [Header("Misc.")]
    [SerializeField] private TargetType currentTargetType = TargetType.NULL;

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


        // target selection panel ref


        // target button references
        UQueryBuilder<Button> buttons = uiDocument.rootVisualElement.Query<Button>(className: "targetButton");

        buttons.ForEach(new List<bool>(), (Button current) =>
        {
            // add button to array
            targetButtons.Add(current);
            return true;
        }); // get buttons added to list

        // loop through buttons and use the function factory
        for (int i = 0; i < targetButtons.Count; i++) 
        {
            targetButtons[i].clicked += MakeTargetSelectionFunc(i);
        }

        targetSelectionVBox = uiDocument.rootVisualElement.Query<VisualElement>("TargetSelectionVBox");
        mainSelectionPanel = uiDocument.rootVisualElement.Query<VisualElement>("TargetPanel");

        // event subscription
        fightButton.clicked += OnFightPressed;
        skillsButton.clicked += OnSkillsPressed;
        itemsButton.clicked += OnItemsPressed;
        fleeButton.clicked += OnFleePressed;


        // if we are the server/host, subscribe to these events
        if (IsHost)
        {
            BattleManager.OnCompletedTurnEndProcessing += TurnEndedHandler;
        }

        if (CheckWeOwnSpecifiedPlayer(startingPlayerID))
        {
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
        currentTargetType = TargetType.ENEMY;
        SetTargetTypeOnServerRPC(currentTargetType);
        Debug.Log("CLIENT: Fight Pressed");
        // clear out previous enemy data
        enemyData.ClearEnemies();
        // get a list of all enemies, add them to the selection listview
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Debug.Log("Found enemies: " + enemies.Length);

        // clear targer button list
        targetButtonsList.ClearTargets();

        foreach (var enemy in enemies)
        {
            var status = targetButtonsList.TryAddTarget(enemy);
            Debug.Log("Could add enemy: " +  status);
        }

        mainSelectionPanel.visible = true;
        targetSelectionVBox.visible = true;
        // go through the buttons and check if they need to be shown
        for (int i = 0; i < targetButtons.Count; i++)
        {
            if (i < targetButtonsList.GetTargetCount())
                targetButtons[i].visible = true; // make it visible
            else
                targetButtons[i].visible = false;
        }
    }

    private bool CheckWeOwnSpecifiedPlayer(ulong startID)
    {
        // use this to check if we need to enable / disable the buttons
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int index = -1;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkObject>().NetworkObjectId == startID)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return false;

        // check we own the player

        return players[index].GetComponent<NetworkObject>().IsOwner;
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
    private void SendTargetRPC(int targetIndex)
    {
        Debug.Log("SERVER: Recieved a target, index: " + targetIndex); 
        switch (currentTargetType)
        {
            case TargetType.PLAYER:
                break;
            case TargetType.ENEMY:
                // enemy is being attacked
                Debug.Log("SERVER: Attacking Enemy (from UI)");
                BattleManager.Singleton.EnemyAttacked(targetIndex);
                break;
        }
    }

    [Rpc(SendTo.Server)]
    private void SetTargetTypeOnServerRPC(TargetType desiredType)
    {
        currentTargetType = desiredType;
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

    [Rpc(SendTo.ClientsAndHost)]
    private void EnableBattleUIRPC(ulong playerID)
    {
        if (CheckWeOwnSpecifiedPlayer(playerID))
        {
            // we own current player, enable
            EnableBattleUI();
        }
    }

    private void DisableBattleUI()
    {
        Debug.Log("CLIENT: Disabling battle UI interaction");

        // disable buttons
        fightButton.enabledSelf = false;
        fleeButton.enabledSelf = false;
        itemsButton.enabledSelf = false;
        skillsButton.enabledSelf = false;

        // go through target list, make it invis
        for (int i = 0; i < targetButtons.Count; i++) 
        {
            targetButtons[i].visible = false;
        }
        // hide targeting panel
        mainSelectionPanel.visible = false;
        targetSelectionVBox.visible = false;

        // hide scroll buttons
        //scrollButtonPanel.visible = false;
    }

    // method to give bindings & prevent late binding
    private Action MakeTargetSelectionFunc(int targetIndex)
    {
        return () => { 
            // send target to server & disable the ui
            SendTargetRPC(targetIndex);
            DisableBattleUI();
        };
    }
}

public enum MenuType
{
    ENEMIES,
    SKILLS,
    ITEMS
}