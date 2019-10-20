using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class handles the abstract parts of a skill and not the specific game mechanics to be game agnostic. It keeps track of the cooldown, and descriptions / images related to the skill
/// </summary>
[System.Serializable]
public class Skill
{   
    /// <summary>
    /// Unique name of the skill
    /// </summary>
    [SerializeField]
    public string SkillName;

    /// <summary>
    /// Information displayed in the tooltip (this is a large box
    /// </summary>
    [SerializeField]
    public string ToolTipText;

    /// <summary>
    /// Cooldown between skill activations
    /// </summary>
    [SerializeField]
    public float Cooldown;

    /// <summary>
    /// Duration the skill is active (recommend putting 0.5s for skills that are "instant")
    /// </summary>
    [SerializeField]
    public float Duration;

    /// <summary>
    /// Icon which denotes this skill
    /// </summary>
    [SerializeField]
    public Sprite Icon;

    public delegate void SkillActivated(Skill _skill);

    /// <summary>
    /// Event triggered when this skill activates
    /// </summary>
    public event SkillActivated SkillActivatedEvent;

    private float mTime = float.MinValue;

    /// <summary>
    /// Activate the skill
    /// </summary>
    /// <returns>false if activation fails</returns>
    public bool Activate()
    {
        if ( (Time.realtimeSinceStartup - mTime) > Cooldown) 
        {
            mTime = Time.realtimeSinceStartup;
            ActivatedEvent();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Test if the skill is ready to activate
    /// </summary>
    /// <returns>true if ready</returns>
    public bool IsReady()
    {
        if ((Time.realtimeSinceStartup - mTime) > Cooldown)
            return true;

        return false;
    }

    /// <summary>
    /// Try to activate the event
    /// </summary>
    private void ActivatedEvent()
    {
        try
        {
            Debug.Log("Activating events");
            if (SkillActivatedEvent != null)
                SkillActivatedEvent(this);
        }
        catch(System.Exception e)
        {
            Debug.Log(string.Format("SkillActivatedEvent failed: {0} at {1}", e.Message, e.StackTrace)); 
        }
    }
    
}

/// <summary>
/// Skillbutton class shows the values from the skill object
/// </summary>
public class SkillButton : MonoBehaviour 
{
    /// <summary>
    /// Skill object this button reflects (can be set via SetSkill)
    /// </summary>
    [Tooltip("Skill object this button reflects (can be set via SetSkill)")]
    public Skill Skill;

    /// <summary>
    /// Container for this skill object UI
    /// </summary>
    [Tooltip("Container for this skill object UI")]
    public Transform Container;

    /// <summary>
    /// Icon sprite layer in UI
    /// </summary>
    [Tooltip("Icon sprite layer in UI")]
    public Image IconSprite;

    /// <summary>
    /// Active effect glow layer in UI
    /// </summary>
    [Tooltip("Active effect glow layer in UI")]
    public Image ActiveGlow;

    /// <summary>
    /// timeout effect layer in UI
    /// </summary>
    [Tooltip("timeout effect layer in UI")]
    public Image TimeOut;

    /// <summary>
    /// Event triggered when the skill is activated
    /// </summary>
    public System.EventHandler Activated;

    private bool mInitialized = false;
    
    /// <summary>
    /// Validates the current selected skill for this button
    /// </summary>
    /// <returns>true if valid</returns>
    bool Validate()
    {
        bool valid = true;
        if (Skill == null)
        {
            valid = false;
            Debug.LogError("Skill is missing");
        }

        if (IconSprite == null)
        {
            valid = false;
            Debug.LogError("IconSprite is missing, assign it in the editor");
        }

        if (ActiveGlow == null)
        {
            valid = false;
            Debug.LogError("ActiveGlow is missing, assign it in the editor");
        }

        if (TimeOut == null)
        {
            valid = false;
            Debug.LogError("TimeOut is missing, assign it in the editor");
        }

        return valid;
    }

    void OnValidate()
    {
        Validate();
    }

    void OnEnable()
    {
        mInitialized = Validate();

        UpdateForm();
    }

    /// <summary>
    /// Set a new skill for this interface
    /// </summary>
    /// <param name="_skill"></param>
    public void SetSkill(Skill _skill)
    {
        if (_skill == null)
        {
            Debug.LogError("SkillButton:SetSkill: Skill is null");
            return; 
        }

        Skill = _skill;

        UpdateForm();
    }

    /// <summary>
    /// Updates the form to show new icons
    /// </summary>
    void UpdateForm()
    {
        IconSprite.sprite = Skill.Icon;
        TimeOut.fillAmount = 0;
        ActiveGlow.enabled = false;
    }

    /// <summary>
    /// Registers when the skill button has been pressed
    /// </summary>
    public void ButtonPressed()
    {
        if (!Skill.IsReady())
            return;
        
        if (Skill.Activate())
        {
            
            ActiveGlow.enabled = true;

            StartCoroutine(SkillActivate());
            
            if (Activated != null) // This feels a bit redundant, but there may be some UI objects that rely on this.
                Activated(this, System.EventArgs.Empty);
        }
    }

    /// <summary>
    /// Activation animation
    /// Sets the fill ammount on the "timeout" layer. Also removes activateion glow when the skill is timed out
    /// </summary>
    IEnumerator SkillActivate()
    {
        float t = 0;

        while (t < Skill.Cooldown)
        {
            yield return new WaitForFixedUpdate();

            if (t > Skill.Duration)
            {
                ActiveGlow.enabled = false;
            }

            TimeOut.fillAmount = (1.0f - t / Skill.Cooldown);

            t += Time.fixedDeltaTime; // This is at the end so we get at least one iteration of t=0;
        }

    }

    /// <summary>
    /// Adds the tooltip
    /// </summary>
    public void OnMouseEnter()
    {
        ToolTip.ShowToolTipStatic(Skill.ToolTipText);
    }

    /// <summary>
    /// Removes the tooltip
    /// </summary>
    public void OnMouseExit()
    {
        ToolTip.HideStatic();
    }

    void Update()
    {
        

    }

    void StartCooldown()
    {

    }
    bool CooldownActive()
    {
        return false;
    }

}
