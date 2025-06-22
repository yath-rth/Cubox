using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Stat Object", order = 0)]
public class upgradeStats : upgrades
{
    public List<Stats> appliedStats = new List<Stats>();
    public List<StatValue> upgrades = new List<StatValue>();

    public List<upgradeStats> requiredUgrade = new List<upgradeStats>();
    public List<upgradeNonStats> requiredNonStatUgrade = new List<upgradeNonStats>();

    public override void DoUpgrade()
    {
        foreach (Stats item in appliedStats)
        {
            foreach (StatValue item1 in upgrades)
            {
                item.unlockUpgrade(item1);
            }
        }
    }
}
