using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Non Stat Object", order = 0)]
public class upgradeNonStats : upgrades
{
    public List<upgradeStats> requiredUgrade = new List<upgradeStats>();
    public List<upgradeNonStats> requiredNonStatUgrade = new List<upgradeNonStats>();

    public override void DoUpgrade()
    {

    }
}
