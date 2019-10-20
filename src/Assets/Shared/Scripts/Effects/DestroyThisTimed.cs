using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys the game object after specified period
/// </summary>
public class DestroyThisTimed : MonoBehaviour
{
    [Tooltip("Time in seconds before object is deleted")]
    public float Time;

    void Start()
    {
        Destroy(gameObject, Time);
    }
}
