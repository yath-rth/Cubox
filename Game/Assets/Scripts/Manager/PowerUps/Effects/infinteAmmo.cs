using UnityEngine;

[CreateAssetMenu(fileName = "Infinte Ammo", menuName = "PowerUp Effects/Infinte Ammo Effect")]
public class infinteAmmo : PowerUpEffects
{
    public override void ApplyEffect(PowerUpScriptableObject powerUp)
    {
        if (Player.playerInstance != null)
        {
            Player.playerInstance.gun.activeGun.infiniteAmmo = true;
        }
    }

    public override void RemoveEffect(PowerUpScriptableObject powerUp)
    {
        if (Player.playerInstance != null)
        {
            Player.playerInstance.gun.activeGun.infiniteAmmo = false;
        }
    }
}