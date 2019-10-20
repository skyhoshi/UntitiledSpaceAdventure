using UnityEngine;
using System.Collections;

/// <summary>
/// UI Bar that displays how much energy the player has remaining. Wraps Healthbar
/// </summary>
public class EnergyBar : MonoBehaviour {

    /// <summary>
    /// Target player to be watched
    /// </summary>
    public Player Player;

    private HealthBar ProgressBar;

    private bool bRoutineRunning = false;

    void OnEnable()
    {
        if (Player == null)
            StartCoroutine(GetPlayer());

        ProgressBar = gameObject.GetComponent<HealthBar>();
    }

    /// <summary>
    /// Waits and searches for the player (sometimes the player is dead and we need to keep checking until the player appears again)
    /// </summary>
    IEnumerator GetPlayer()
    {
        bRoutineRunning = true;
        while (Player == null)
        {
            GameObject g = GameObject.FindGameObjectWithTag("Player");
            if (g != null)
            {
                Player = g.GetComponent<Player>();
                ProgressBar.SetWatchValue(Player, "Energy", Player.MaxEnergy);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        bRoutineRunning = false;
    }
    
    void FixedUpdate()
    {
        if (Player == null && bRoutineRunning == false)
            StartCoroutine(GetPlayer());
    }


}
