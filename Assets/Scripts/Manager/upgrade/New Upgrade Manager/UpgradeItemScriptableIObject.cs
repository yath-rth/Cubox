using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "UpgradeItem", menuName = "Upgrade Items")]
public class UpgradeItemScriptableIObject : ScriptableObject
{
    public Sprite itemImage;// Image of the item to be displayed in the shop
    public Color itemImageTint = Color.white; // Tint color for the item image
    public string itemName;// Name of the item to be displayed in the shop
    public int itemPrice;// Price of the item in the shop
    public string itemDescription;// Description of the item to be displayed in the shop
}

[Serializable]
public class UpgradeItem
{
    public UpgradeItemScriptableIObject data;
    public bool isPurchased = false; // Flag to check if the item is purchased
    public int itemQuantity = 1;// Quantity of the item available in the shop
    public UpgradeItemEffects[] effects;// Event to register all the fuctions which need to be called on purchase of item
}