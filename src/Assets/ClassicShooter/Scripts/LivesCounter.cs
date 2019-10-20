using UnityEngine;
using System.Collections;
using StageShooter;

/// <summary>
/// Removes a life from the counter when it recieves the player dead event.
/// </summary>
public class LivesCounter : MonoBehaviour {

    public LifeSprite[] Images;
    public LevelController Controller;

    void OnEnable()
    {
        Controller.PlayerDeadEvent += new System.EventHandler(Controller_PlayerDeadEvent);
        Controller.PlayerRespawnEvent += new System.EventHandler(Controller_PlayerRespawnEvent);
    }

    void Start()
    {
        InitLifeDisplay();
    }

    void Controller_PlayerRespawnEvent(object sender, System.EventArgs e)
    {
        UpdateLifeDisplay();
    }

    void OnDisable()
    {
        Controller.PlayerDeadEvent -= Controller_PlayerDeadEvent;
        UpdateLifeDisplay();
    }

    void Controller_PlayerDeadEvent(object sender, System.EventArgs e)
    {
        UpdateLifeDisplay();
    }

    /// <summary>
    /// Sets up the life display
    /// </summary>
    public void InitLifeDisplay()
    {
        Debug.Log("Init display Num lives = " + Controller.NumLives);
        for (int i = 0; i < Images.Length; i++)
        {

            if (i < Controller.NumLives)
                Images[i].EnableLifeSprite();
            else
            {
                Images[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the visible lives when the game state changes
    /// </summary>
    public void UpdateLifeDisplay()
    {
        Debug.Log("Num lives = " + Controller.NumLives);
        for (int i = 0; i < Images.Length; i++)
        {
            
            if (i < Controller.NumLives)
                Images[i].EnableLifeSprite();
            else
            {
                if (Images[i].gameObject.activeSelf == true)
                    Images[i].DisableLifeSprite();
            }
        }
    }

}
