using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Skillbutton tooltip. Displays a large rectangular MMO-style tooltip. This is a singleton.
/// </summary>
public class ToolTip : MonoBehaviour {

    /// <summary>
    /// Main text body
    /// </summary>
    [Tooltip("Main text body")]
    public Text BodyText;

    /// <summary>
    /// This object
    /// </summary>
    [Tooltip("This object")]
    public GameObject Self; 

    /// <summary>
    /// True if initialized properly
    /// </summary>
    private bool mInitialized = false;

    /// <summary>
    /// Sets whether this tooltip is hidden
    /// </summary>
    private bool bHidden = true;

    /// <summary>
    /// Singleton
    /// </summary>
    private static ToolTip Instance;

    public void Start()
    {
        OnEnable();
    }

    public void OnEnable()
    {
        OnValidate();

        if (bHidden)
            Hide();
        else
            Show();

        Instance = this;
    }

    public void OnValidate()
    {
        if (Self == null)
        {
            Debug.Log("Self is null, assign it in editor");
            return;
        }

        if (BodyText == null)
        {
            Debug.Log("BodyText is null, assign it in editor");
            return;
        }

        mInitialized = true;
    }

    /// <summary>
    /// Show tool tip. Displays the single tooltip instance with the chosen body text
    /// </summary>
    /// <param name="_bodyText">Body text</param>
    public static void ShowToolTipStatic(string _bodyText)
    {
        Instance.ShowToolTip(_bodyText);
    }

    /// <summary>
    /// Show tool tip. Displays tooltip with the chosen body text
    /// </summary>
    /// <param name="_bodyText">Body text</param>
    public void ShowToolTip(string _bodyText)
    {
        if (!mInitialized)
            return;

        BodyText.text = _bodyText;
        Show();
    }

    /// <summary>
    /// Shows the UI
    /// </summary>
    protected void Show()
    {
        if (!mInitialized)
            return;

        bHidden = false;
        Self.SetActive(true);
    }
    
    /// <summary>
    /// Hides the tooltip instance
    /// </summary>
    public static void HideStatic()
    {
        Instance.Hide();
    }

    /// <summary>
    /// Hides the UI
    /// </summary>
    public void Hide()
    {
        if (!mInitialized)
            return;

        bHidden = true;
        Self.SetActive(false);
    }

    void Update()
    {
        Vector3 offset = Vector3.zero;
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            offset.x = rt.rect.width/2 + 10;
            offset.y = rt.rect.height / 2 +10;
        }
        transform.position = Input.mousePosition + offset;
    }
}
