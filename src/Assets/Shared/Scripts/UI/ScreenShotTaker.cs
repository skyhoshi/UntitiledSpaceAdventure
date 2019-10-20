using UnityEngine;
using System.Collections;

/// <summary>
/// Takes screenshots and saves them in the root directory of the game when the P key is pressed
/// This is intended for development use - it may cause problems on mobile.
/// </summary>
public class ScreenShotTaker : MonoBehaviour {

    public string FileName;
    public bool bDontDestroyOnLoad = true;
    static int numShots = 0;

    void Awake()
    {
        if (bDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Takes screen shot on late update (after everything is rendered)
    /// </summary>
    void LateUpdate()
    {
        // This is done in late update to make sure everything is drawn. 
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(FileName + "." + numShots + ".jpg");
            numShots++;
        }
    }
}
