using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Effects/Damage Increase", order = 0)]
public class StatIncreaseEffect : UpgradeItemEffects
{
    public override void Apply(UpgradeItem item)
    {
        foreach (Stats Stat in statsToAffect)
        {
            Stat.unlockUpgrade(new StatValue(stat.stat, stat.value)); // Ensure the damage stat is unlocked
        }

        Debug.Log($"Increased damage by {stat.value} for {item.data.itemName}");
    }
}
