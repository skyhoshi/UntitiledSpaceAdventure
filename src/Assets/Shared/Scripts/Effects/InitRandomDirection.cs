using UnityEngine;
using System.Collections;

/// <summary>
/// Initializes an object to face a random direction when it spawns 
/// </summary>
public class InitRandomDirection : MonoBehaviour {
	
	void Start () 
    {
        float dir = Random.Range(0, 360);
        transform.Rotate(0, 0, dir);
	}
	
}
