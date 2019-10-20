using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StageShooter;

public class HUD : MonoBehaviour {

    /// <summary>
    /// Panel used to comunicate game state
    /// </summary>
    public Transform MessagePanel;
    /// <summary>
    /// Message panels body text
    /// </summary>
    public Text MessagePanelText;

    /// <summary>
    /// Current score
    /// </summary>
    /// 
    public Text Score;
    /// <summary>
    /// Remaining game time 
    /// </summary>
    public Text Time;

    /// <summary>
    /// The level controler
    /// </summary>
    public LevelController Controller;

    /// <summary>
    /// Checks the object has been correctly set up in the scene
    /// </summary>
    void OnValidate()
    { 
        if (Controller == null)
            Debug.LogError(string.Format("{0}: LevelController is missing, assign it in editor",name));

        if (Score == null)
            Debug.LogError(string.Format("{0}: Score is missing, assign it in editor", name));

        if (Time == null)
            Debug.LogError(string.Format("{0}: Time is missing, assign it in editor", name));
    }


    void Update()
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(Controller.GetRemainingTime());
        
        // Display the remianing game time we got from the controller
        Time.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        
        Score.text = Controller.PlayerScore.ToString();
    }
}
