using UnityEngine;
using System.Collections;

/// <summary>
/// Default for a damage dealer.
/// Base class for damage dealer types
/// </summary>
[System.Serializable]
public class DamageDealer : MonoBehaviour {

    /// <summary>
    /// Default ammount of damage dealt. The interaction is handled in destructable
    /// </summary>
    [UnityEngine.SerializeField]
    public float Damage;


    [UnityEngine.SerializeField]
    public bool DestroyOnHit = true;
}
