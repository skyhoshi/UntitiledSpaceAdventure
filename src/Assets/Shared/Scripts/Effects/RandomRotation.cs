using UnityEngine;
using System.Collections;

/// <summary>
/// Initializes an object to face a random direction when it spawns
/// Usefull for objects that explode into multiple debris peices
/// </summary>
public class RandomRotation : MonoBehaviour {

	void Start() 
    {
        gameObject.transform.Rotate(0, 0, Random.Range(0, 360));
	}
	
    
}
