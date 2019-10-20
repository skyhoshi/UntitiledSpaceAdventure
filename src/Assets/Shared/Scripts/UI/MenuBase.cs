using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class that handles the basics of showing and hiding a menu and launching events for those
/// </summary>
public class MenuBase : MonoBehaviour {

    protected bool      mOpen = false;
    private bool        mInit = false;

    /// <summary>
    /// Panel we are showing and hiding
    /// </summary>
    [Tooltip("Panel we are showing and hiding")]
    public Transform    MenuPanel;

    /// <summary>
    /// Whether this menu should start open or closed when the scene is run
    /// </summary>
    [Tooltip("Whether this menu should start open or closed when the scene is run")]
    public bool         StartOpen = false;

    /// <summary>
    /// Triggered when the menu opens
    /// </summary>
    public event EventHandler MenuOpened;

    /// <summary>
    /// Triggered when the menu closes
    /// </summary>
    public event EventHandler MenuClosed;

    public bool MenuOpen
    {
        get { return mOpen; }
    }

    void OnEnable()
    {
        if (MenuPanel == null)
        {
            Debug.LogError(string.Format("{0}: MenuPanel is null, please assign in editor", name));
            return;
        }

        // If this interface is set to start open, set mOpen to true so it gets opened on enable
        if (mInit == false && mOpen == false && StartOpen == true)
            mOpen = true;

        // If the panel was disabled/enabled as part of the hierachy we want it to remain in the open / close state it was previously.
        if (mOpen == true)
            MenuPanel.gameObject.SetActive(true);
        else
            MenuPanel.gameObject.SetActive(false);

        mInit = true;
    }

    /// <summary>
    /// Opens the interface, triggers the open event
    /// </summary>
    public void Show()
    {
        if (mInit == false) return;

        mOpen = true;
        MenuPanel.gameObject.SetActive(true);

        if (MenuOpened != null)
            MenuOpened(this, System.EventArgs.Empty);
    }

    /// <summary>
    /// closes the interface, triggers the close event
    /// </summary>
    public void Hide()
    {
        if (mInit == false) return;

        mOpen = false;
        MenuPanel.gameObject.SetActive(false);

        if (MenuClosed != null)
            MenuClosed(this, System.EventArgs.Empty);
    }
    
	
}
