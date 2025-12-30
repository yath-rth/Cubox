using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<ScriptableObject> objects;

    void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }

    public void ItemSelected(InventoryItemSO item)
    {
        item.EquipItem();
    }

    public void AddItem(ScriptableObject item)
    {
        if (objects.Contains(item))
        {
            //TODO 
            //Add functionality for what happens when you pick up more than 1 of the same item
        }
        else
        {
            var so = item.GetAs<InventoryItemSO>();

            if (so != null)
            {
                so.AddItem();
            }
        }
    }
}
