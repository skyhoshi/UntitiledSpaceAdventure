using UnityEngine;
using System.Collections;

/// <summary>
/// Detach this game object from it's parent, do not reparent
/// </summary>
public class Detach : MonoBehaviour {

    void Start()
    {
        transform.parent = null;
    }
}
