using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade", order = 0)]
public abstract class upgrades : ScriptableObject
{
    public string upgradename;
    public string description;

    public UpgradeTypes type;
    public List<gunType> guntype = new List<gunType>();

    public Sprite icon;
    public int useORdont;

    public abstract void DoUpgrade();
}

[Serializable]
public enum UpgradeTypes
{
    damageMultiplier,
    speedMultiplier,
    regenration,
    muzzleUnlock,
}