using UnityEngine;
using System.Collections;



/// <summary>
/// Follows a world object while in the 3d canvas. Used to make UI elements follow their respective game objects
/// Example usage: Hit point bars that follow the unit
/// </summary>
//[ExecuteInEditMode]
public class FollowWorldObject : MonoBehaviour
//#if UNITY_EDITOR 
//    , ISerializationCallbackReceiver
//#endif
//{
{

    public void OnBeforeSerialize()
    {
        Debug.Log("before called!$%^$");
    }
    public void OnAfterDeserialize()
	{
		Debug.Log("Serialize called");
	}
    void OnValidate()
    {
        EditorUpdate();
    }

    public Transform Target;
    public Vector3 Offset;

    //[ExecuteInEditMode]
    void EditorUpdate()
    {
        if (Target != null)
            Offset = transform.position - Target.position;
    }

    /// <summary>
    /// Update the position
    /// </summary>
    void Update()
    {        
        transform.position = Target.position + Offset;
    }

}
