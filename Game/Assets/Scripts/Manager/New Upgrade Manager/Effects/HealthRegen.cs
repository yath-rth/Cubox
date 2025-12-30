using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Effects/Health Regen", order = 0)]
public class HealthRegen : UpgradeItemEffects
{
    public override void Apply(UpgradeItem item)
    {
        if (Player.playerInstance != null)
        {
            Player.playerInstance.shouldRegen = true;
        }
    }
}
