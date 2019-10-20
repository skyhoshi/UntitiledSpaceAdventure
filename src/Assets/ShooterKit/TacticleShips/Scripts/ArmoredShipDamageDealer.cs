using UnityEngine;
using System.Collections;

/// <summary>
/// This is an example of a different rule set for dealing damage
/// In this case ships have Shields armor and hull. Shields always take damage first then armor then the hull. If the hull reaches 0 then the ship is destroyed.
/// The exiting part is the exeptions. Different weapons may damage Shields Armor and hull differently.  
/// </summary>
[System.Serializable]
public class ArmoredShipDamageDealer : DamageDealer
{
    [UnityEngine.SerializeField]
    public string WeaponType;

    // Normal weapons do 1x damage. You can set the multiplier for weapons that do extra damage to specific layers.
    [UnityEngine.SerializeField]
    public float DamageMultiplierShields = 1;
    [UnityEngine.SerializeField]
    public float DamageMultiplierArmor = 1;
    [UnityEngine.SerializeField]
    public float DamageMultiplierHull = 1;

    // Damage is done to shields first by default. Remaining damage drops to the next layer. 
    // These values represent where the damage is placed first. 0,1,0 would mean that the weapon bypasses shields and hits armor first. 
    // 1, 1, 1, would be a weapon that deals it's damage evenly split between all three layers (while the first layers remain).
    [UnityEngine.SerializeField]
    public float DamageDistributionShields = 1;
    [UnityEngine.SerializeField]
    public float DamageDistributionArmor = 0;
    [UnityEngine.SerializeField]
    public float DamageDistributionHull = 0;
    void OnValidate()
    {
        //base.OnValidate();

    }
}
