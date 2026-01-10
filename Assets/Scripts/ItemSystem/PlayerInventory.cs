using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerInventory : NetworkBehaviour
{
    // inventory component that will be attached to a player

    [SerializeField] private List<AItem> itemList = new();
    private NetworkVariable<int> itemCount = new();


    // methods for interacting with inventory

    public int GetItemCount()
    {
        return itemCount.Value;
    }
}
