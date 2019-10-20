using UnityEngine;
using System.Collections;

/// <summary>
/// Sometimes destroys this object. Use this sparingly because it's a costly way to have random debris!
/// </summary>
public class InitDestroyRandom : MonoBehaviour
{
    /// <summary>
    /// Chance object is destroyed on start
    /// </summary>
    [Tooltip("Chance object is destroyed on start")]
    public float DestroyChance = 0.5f;

    void OnValidate()
    {
        DestroyChance = Mathf.Clamp(DestroyChance, 0, 1);
    }

    void Start()
    {
        if (Random.Range(0.0f, 1.0f) < DestroyChance)
            Destroy(gameObject);
    }
}
