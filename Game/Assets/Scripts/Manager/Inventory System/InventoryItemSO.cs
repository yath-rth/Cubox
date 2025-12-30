using System;
using UnityEngine;

public interface InventoryItemSO
{
    public string displayName { get; }
    public Sprite image { get; }
    public int quantity{ get; }

    public abstract void EquipItem();
    public abstract void AddItem();
}