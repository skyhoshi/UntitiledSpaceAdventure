using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script that looks for a game object of a specific tag to target. Sends an event when the target changes.
/// 
/// </summary>
public class AITarget : MonoBehaviour {

    private bool isValid = false;
    private Transform mTarget;

    /// <summary>
    /// Get set the AI unit's target. Triggers TargetChangedEvent on set
    /// </summary>
    public Transform Target
    {
        get 
        {
            if (isValid == true && mTarget == null)
            {
                isValid = false;
                Target = null;
            }
            return mTarget; 
        }
        set 
        {
            mTarget = value;
            if (mTarget != null)
                isValid = true;

            if (TargetChangedEvent != null)
                TargetChangedEvent();
        } 
    }

    /// <summary>
    /// Removes the target from the AI unit if the target dead event is recieved
    /// </summary>
    /// <param name="_damageEventArgs">type DamageEventArgs</param>
    void Target_DeadEvent(System.EventArgs _damageEventArgs)
    {
        Target = null;    
    }

    /// <summary>
    /// Tag that targeter searches for
    /// </summary>
    [Tooltip("Tag that targeter searches for")]
    public string ObjectTag;

    void OnEnable()
    {
        if (mTarget == null)
            StartCoroutine(AquireTargetWithTag(ObjectTag));

        TargetChangedEvent += new TargetChanged(AITarget_TargetChangedEvent);
    }

    /// <summary>
    /// Begins aquiring a new target if the previously aquired target was null
    /// </summary>
    void AITarget_TargetChangedEvent()
    {
        //Debug.Log("My target changed -----------------");
        if (mTarget == null)
            StartCoroutine(AquireTargetWithTag(ObjectTag));
    }

    /// <summary>
    /// Find's the first target in the game object list with specified tag
    /// </summary>
    /// <param name="_tag">Object tag of target</param>
    /// <returns></returns>
    IEnumerator AquireTargetWithTag(string _tag)
    {
        while (mTarget == null)
        {
            GameObject g = GameObject.FindGameObjectWithTag(_tag);
            if (g != null)
                Target = g.transform;
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    public delegate void TargetChanged();
    public event TargetChanged TargetChangedEvent;

    /// <summary>
    /// Returns whether the unit currently 
    /// </summary>
    /// <returns>bool</returns>
    public bool HasTarget()
    {
        bool rv = (Target != null);
        return rv;
    }
}
