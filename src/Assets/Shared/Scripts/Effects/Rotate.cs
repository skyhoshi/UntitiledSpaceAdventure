using UnityEngine;
using System.Collections;

/// <summary>
/// Rotates game object by specified value continuously
/// </summary>
public class Rotate : MonoBehaviour {

    /// <summary>
    /// rotation to apply
    /// </summary>
    [Tooltip("rotation to apply")]
    public Vector3 rotation = new Vector3(0, 0, 0.02f);

	void Update () {    
            gameObject.transform.Rotate(rotation);    
	}
}
