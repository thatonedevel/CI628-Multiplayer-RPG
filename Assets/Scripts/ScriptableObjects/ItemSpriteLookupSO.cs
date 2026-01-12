using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpriteLookupSO", menuName = "Scriptable Objects/ItemSpriteLookupSO")]
public class ItemSpriteLookupSO : ScriptableObject
{
    [SerializeField] private SpriteKVPairing[] spriteList;

    public Sprite GetSpriteByID(string id)
    {
        // use linear search as its easiest to do rn
        // does lead to O(n) complexity but shouldn't be too bad
        // returns null if specified id isn't in the array

        Sprite foundSprite = null;

        for (int i = 0; i < spriteList.Length; i++)
        {
            if (spriteList[i].itemID.Equals(id))
            {
                foundSprite = spriteList[i].itemSprite;
                break;
            }
        }

        return foundSprite;
    }

    public Sprite GetSpriteAtIndex(int index)
    {
        return spriteList[index].itemSprite;
    }
}


// helper class to serialise the data
[Serializable]
public class SpriteKVPairing
{
    public string itemID;
    public Sprite itemSprite;
}