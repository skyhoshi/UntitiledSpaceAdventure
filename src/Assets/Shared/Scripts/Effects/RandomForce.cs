using UnityEngine;
using System.Collections;


/// <summary>
/// Gives an object a random force when it starts  works on 2d and 3d physics 
/// Usefull for objects that explode into multiple debris peices
/// </summary>
public class RandomForce : MonoBehaviour {

    /// <summary>
    /// Maximum force to apply
    /// </summary>
    [Tooltip("Maximum force to apply")]
    public Vector3 ForceMax;

    void Start()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
            rb2d.AddForce(new Vector2(Random.Range(-ForceMax.x, ForceMax.x), Random.Range(-ForceMax.y, ForceMax.y)));
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(new Vector3(Random.Range(-ForceMax.x, ForceMax.x), Random.Range(-ForceMax.y, ForceMax.y), Random.Range(-ForceMax.z, ForceMax.z) ));

    }
}
