using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Globalization;

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
                    UpdateItemNameListRPC(MiscUtils.ConvertStringArray(GetItemNamesFromInv()));
                }
            }
        }
    }

    private string[] GetItemNamesFromInv()
    {
        string[] names = new string[itemList.Count];

        for (int i = 0; i < itemList.Count; i++)
        {
            names[i] = itemList[i].ItemName;
        }

        return names;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateItemNameListRPC(int[] nameData)
    {
        // can't send a list of names
        itemNameList.Clear();
        // convert the nameData back into the array of strings
        string[] names = MiscUtils.DecodeDelimitedCharArrAsString(nameData);

        for (int i = 0; i < names.Length; i++) 
        {
            itemNameList.Add(names[i]);
        }
    }
}
