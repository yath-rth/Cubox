using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;

        [Header("UI Elements")]
    [SerializeField] GameObject itemTileUI;
    [SerializeField] Transform itemTileParent;

    void Awake()
    {
        inventory = Inventory.instance;

        for (int i = 0; i < inventory.objects.Count; i++)
        {
            GameObject tile = Instantiate(itemTileUI, itemTileParent);
            if (tile != null)//Null check to avoid null pointer error
            {
                if (inventory.objects[i] != null)
                {
                    var so = inventory.objects[i].GetAs<InventoryItemSO>();

                    if (so != null)
                    {
                        Debug.Log("name: " + so.displayName);
                        tile.GetComponent<InventoryItemTile>().setupTile(so, inventory.ItemSelected);
                    }
                }
            }
        }
    }
}
