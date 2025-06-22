using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Effects/Muzzle Unlock", order = 1)]
public class MuzzleUnlockEffect : UpgradeItemEffects
{
    public int numberOfMuzzlesToUnlock;

    public override void Apply(UpgradeItem item)
    {
        if (Player.playerInstance != null)
        {
            Player.playerInstance.gun.muzzleUnlock(numberOfMuzzlesToUnlock);
        }
    }
}
