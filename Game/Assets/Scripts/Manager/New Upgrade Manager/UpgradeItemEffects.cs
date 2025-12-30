using UnityEngine;
public abstract class UpgradeItemEffects : ScriptableObject
{
    public StatValue stat;
    public Stats[] statsToAffect; // Array of stats that will be affected by this effect
    public abstract void Apply(UpgradeItem item);
}


/*
    * This script defines an abstract class UpgradeItemEffects that inherits from ScriptableObject.
    * It is used to create upgrade effects that can be applied to upgrade items in a game.
    * The Apply method must be implemented by any class that inherits from UpgradeItemEffects.
    */

// Example of a derived class that implements the Apply method
/* 

using UnityEngine;

[CreateAssetMenu(menuName = <<Name of the effect>>)] 
public class <<Name of the effect>> : UpgradeItemEffects
{
    public override void Apply(UpgradeItem item)
    {
        // Implement the effect logic here    
    }
}

*/