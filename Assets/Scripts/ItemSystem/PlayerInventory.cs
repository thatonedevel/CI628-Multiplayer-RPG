using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerInventory : NetworkBehaviour
{
    // inventory component that will be attached to a player

    [SerializeField] private List<AItem> itemList = new();
    private List<string> itemNameList = new();
    private AnticipatedNetworkVariable<int> itemCount = new();


    // methods for interacting with inventory

    public int GetItemCount()
    {
        return itemCount.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            // is it an item?
            if (other.CompareTag("Item"))
            {
                // make new item from id & add it to the inventory
                AItem newItem = ItemFactory.CreateItem(other.GetComponent<WorldItem>().GetItemID());
                
                // check if inventory is not full
                if (itemList.Count < 8)
                {
                    itemList.Add(newItem);
                    itemCount.Anticipate(itemList.Count);
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateItemNameList()
    {

    }
}
