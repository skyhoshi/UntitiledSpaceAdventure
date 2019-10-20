using UnityEngine;
using System.Collections;

/// <summary>
/// Makes object move as a paralax relative to a target
/// </summary>
public class Paralax : MonoBehaviour
{
    /// <summary>
    /// Target to move with
    /// </summary>
    public Transform Target;

    /// <summary>
    /// Distance from object (0 is same plane 0.99999 is very far away
    /// </summary>
    [Tooltip("Distance from object (0 is same plane 0.99999 is very far away")]
    public float Distance;

    /// <summary>
    /// Offset from the object
    /// </summary>
    [Tooltip("Offset from the object")]
    public Vector3 OffsetPosition;
    
    void Start()
    {
        OffsetPosition = gameObject.transform.position;
        Position = new Vector3(0, 0, 0);
        Position.z = OffsetPosition.z;
    }

    /// <summary>
    /// Objects current position
    /// </summary>
    public Vector3 Position;
    
    void Update()
    {
        Position.x = Target.position.x * Distance + OffsetPosition.x;
        Position.y = Target.position.y * Distance + OffsetPosition.y;
        gameObject.transform.position = Position;   
    }
}