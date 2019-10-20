using UnityEngine;
using System.Collections;

/// <summary>
/// Effect script, when the object is moving the effect is shown. When the object stops moving the effect is hidden.
/// </summary>
public class ThrusterScript : MonoBehaviour {

	private AdvancedPlayerMovement Mover;

    /// <summary>
    /// The effect to show and hide.
    /// </summary>
    public GameObject Thruster;

    private bool mInit = false;

    void OnEnable()
    {
        if (mInit == false)
            Init();       
    }

    /// <summary>
    /// Initilizes the game object
    /// </summary>
    void Init()
    {
        if (Thruster == false)
        {
            Debug.LogError("Thruster has not been assigned");
            return;
        }

        Mover = this.GetComponent<AdvancedPlayerMovement>();
        if (Mover == null)
        {
            Debug.LogError("PlayerMovement component is missing");
            return;
        }

        mInit = true;
    }

    void Update () 
    {
	    if (mInit == false)
            return;

        if (Mover.IsMoving == true)
            Thruster.SetActive(true);
        else
            Thruster.SetActive(false);

	}
}
