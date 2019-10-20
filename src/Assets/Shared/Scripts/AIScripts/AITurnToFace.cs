using UnityEngine;
using System.Collections;
using Utils;

/// <summary>
/// Turns an AI unit to face it's currently selected target
/// </summary>
[RequireComponent(typeof(AITarget))]
public class AITurnToFace : MonoBehaviour {

    protected AITarget mTarget;

    [Tooltip("Speed at which the AI Unit rotates")]
    public float RotationSpeed;

    [Tooltip("Difference in between facing angle and the target before the unit starts to rotate (stops dithering)")]
    public float RotationThreshold;

    protected void OnEnable()
    {
        mTarget = gameObject.GetComponent<AITarget>();
    }

    protected void FixedUpdate()
    {
        if (mTarget.HasTarget() == false)
            return;
        
        Vector2 Difference = (Vector2)transform.position - (Vector2)mTarget.Target.position;

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;

        float rotation = Utilities.GetAngleTurnToFace(angle, transform.eulerAngles.z, RotationThreshold, RotationSpeed);

        transform.Rotate(0, 0, rotation);

    }	
}