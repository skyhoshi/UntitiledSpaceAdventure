using UnityEngine;
using System.Collections;

/// <summary>
/// Top level game controller. Singleton
/// </summary>
public class GameController : MonoBehaviour {

    /// <summary>
    /// Self Instance
    /// </summary>
    public static GameController Instance;

    static int id = 0;
    public int myID = -1;
    void Awake()
    {
        myID = id;
        if (Instance != null)
        {
            if (Instance != this)
            {
                Debug.Log("Destroying id = " + id.ToString());
                Destroy(this.gameObject);
                return;
            }

        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        TimeScale = Time.timeScale;
        Debug.Log("id awake = " + id.ToString() + " timescale = " + TimeScale.ToString());

        mCurrentLevel = Application.loadedLevelName;
        id++;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    
    private string mCurrentLevel;

    /// <summary>
    /// Restarts the current level
    /// </summary>
    public void RestartLevel()
    {
        Application.LoadLevel(mCurrentLevel);
    }


    private float TimeScale;

    /// <summary>
    /// Pauses the game using timescale
    /// </summary>
    public void Pause()
    {
        
        Time.timeScale = 0;
        //if (GamePausedEvent != null)
        //{
        //    GamePausedEvent(this, null);
        //}

    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        
        Time.timeScale = TimeScale;
        Debug.Log("Resumeing " + Time.timeScale.ToString());
        //if (GameResumedEvent != null)
        //{
        //    GameResumedEvent(this, null);
        //}
    }
}
