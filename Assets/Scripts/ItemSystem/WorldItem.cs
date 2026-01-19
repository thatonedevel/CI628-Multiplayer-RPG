using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldItem : NetworkBehaviour
{
    [SerializeField] private string Item_ID;
    [SerializeField] private GameObject itemShadow;
    [SerializeField] private ItemSpriteLookupSO itemLookup;
    [SerializeField] SpriteRenderer itemRenderer;

    private void Start()
    {
        // do this on client side
        if (IsClient || IsHost)
        {
            // use the item id to determine sprite
            itemRenderer.sprite = itemLookup.GetSpriteByID(Item_ID);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            RaycastHit hit;
            // use raycast to determine floor height

            // https://discussions.unity.com/t/a-specific-override-of-physics-raycast-doesnt-work/845798/2
            // need to use named params due to overrides being weird
            bool didHit = Physics.Raycast(origin: transform.position, direction:Vector3.down, hitInfo: out hit);

            if (didHit)
            {
                // update shadow y pos
                itemShadow.transform.position = new Vector3(itemShadow.transform.position.x, hit.point.y, itemShadow.transform.position.z);
            }
        }
    }

    public string GetItemID()
    {
        return Item_ID;
    }
}
