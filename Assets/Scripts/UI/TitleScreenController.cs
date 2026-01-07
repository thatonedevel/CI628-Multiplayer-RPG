using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private ManagerSpawner managerSpawner;

    [Header("Scriptable Objects for GUI bindings")]
    [SerializeField] private ErrorMessageData errormessageDataInstance;

    // UI references
    private VisualElement UIRoot;
    // main ui buttons
    private Button hostButton;
    private Button joinButton;
    private Button settingsButton;
    private Button quitButton;

    // error dialogue ui
    private VisualElement errorDialogue;
    private Button errorDialogueOkButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get ui references
        UIRoot = GetComponent<UIDocument>().rootVisualElement;

        // get other elements through queries
        // main buttons
        hostButton = UIRoot.Query<Button>("HostButton");
        joinButton = UIRoot.Query<Button>("JoinButton");
        settingsButton = UIRoot.Query<Button>("SettingsButton");
        quitButton = UIRoot.Query<Button>("QuitButton");

        // error dialogue
        errorDialogue = UIRoot.Query<VisualElement>("ErrorWindow");
        errorDialogueOkButton = UIRoot.Query<Button>("ErrExitBtn");

        // subscribe to button events
        hostButton.clicked += OnHostGameClicked;
        joinButton.clicked += OnJoinGameClicked;
        settingsButton.clicked += OnSettingsClicked;
        quitButton.clicked += OnQuitGameClicked;
        errorDialogueOkButton.clicked += OnErrorMessageOkClicked;
    }

    // button functions
    private void OnHostGameClicked()
    {
        bool success = NetworkManager.Singleton.StartHost();

        if (success)
        {
            // run the manager spawn
            managerSpawner.TrySpawningManagers();
            // load into the lobby scene
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        else
        {
            ShowErrorMessage("Could not host game");
        }
    }

    private void OnJoinGameClicked()
    {
        // start a client instance
        bool success = NetworkManager.Singleton.StartClient();

        if (success)
        {
            Debug.Log("CLIENT: Succesfully Started client-side instance");
        }
        else
        {
            ShowErrorMessage("Could not join game");
        }
    }

    private void OnSettingsClicked()
    {
        // TODO: OPEN GAME SETTINGS ONCE THIS IS SET UP
    }

    private void OnQuitGameClicked()
    {
        // just call main quit method as all net objects should be despawned already
        Application.Quit();
    }

    private void OnErrorMessageOkClicked()
    {
        errorDialogue.visible = false;
    }

    private void ShowErrorMessage(string message)
    {
        errormessageDataInstance.SetError(message);
        // show the window
        errorDialogue.visible = true;
    }
}
