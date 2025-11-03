using UnityEngine;
using Unity.Netcode;

public class GameController : NetworkBehaviour
{
    // purely server / host side game controller class

    // reference to party - state is on a per-player basis

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameController Singleton;
    public NetworkVariable<bool> inBattle = new NetworkVariable<bool>(false);

    void Start()
    {
        if (IsServer)
        {
            // create singleton
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // function that runs on a player state change
    private void OnAnyPlayerStateChange(PlayerState newState)
    {
        switch(newState)
        {
            case PlayerState.BATTLE:
                inBattle.Value = true;
                break;
            case PlayerState.MOVING:
                inBattle.Value = false;
                break;
        }
    }
}
