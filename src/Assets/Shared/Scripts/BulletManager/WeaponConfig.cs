using UnityEngine;
using System.Collections;

/// <summary>
/// Script that attaches to a prefab to allow setting up a weapon
/// </summary>
[System.Serializable]
public class WeaponConfig : MonoBehaviour {

    /// <summary>
    /// The configured weapon
    /// </summary>
    public Weapon mWeapon = new Weapon();

    void OnValidate()
    {
    }

    /// <summary>
    /// Gets the configured weapon
    /// </summary>
    /// <returns>Weapon</returns>
    public Weapon GetWeapon()
    {
        return mWeapon;
    }
}
