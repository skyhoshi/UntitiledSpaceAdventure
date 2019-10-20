using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FlashTargets
{
    None, // Default option for instantiation
    MeshRenderer,
    SpriteRenderer,
    Image,

}

/// <summary>
/// Causes the object to flash when hit. 
/// Requires shader with "_TintColor" or a sprite type rendrer
/// </summary>
public class Flash : MonoBehaviour
{

    /// <summary>
    /// Set which renderer to change
    /// </summary>
    private Renderer mRenderer;
    private SpriteRenderer mSpriteRenderer;
    private Image mImage;

    /// <summary>
    /// Flash colour
    /// </summary>
    [Tooltip("Flash colour")]
    public Color FlashColour;

    /// <summary>
    /// How quickly the object lights up        
    /// </summary>
    [Tooltip("How quickly the object lights up")]
    public float Attack;

    /// <summary>
    /// How long it stays lit
    /// </summary>
    [Tooltip("How long it stays lit")]
    public float Sustain;

    /// <summary>
    /// Number of repeats
    /// </summary>
    [Tooltip("Number of repeats")]
    public float Echo;

    /// <summary>
    /// How quickly it returns to its original colour
    /// </summary>
    [Tooltip("How quickly it returns to its original colour")]
    public float Drop;

    /// <summary>
    /// Shader variable to alter (not used for SpriteRenderers
    /// </summary>
    [Tooltip("Shader variable to alter (not used for SpriteRenderers")]
    public string ColourName = "_TintColor";

    /// <summary>
    /// Set the target renderer to apply the flash effect to
    /// </summary>
    [Tooltip("Set the target renderer to apply the flash effect to")]
    public FlashTargets RendererType;
    

    // Private cached vars
    private Color mColourCached;
    private Color CurrentColour;
    private bool bCached = false;

    public event System.EventHandler FlashCompleteEvent;

    void OnValidate()
    {
        Validate();
    }

    /// <summary>
    /// Validates the flash component
    /// </summary>
    /// <returns>bool valid</returns>
    bool Validate()
    {
        if (RendererType == FlashTargets.Image)
            mImage = GetComponent<Image>();
        else if (RendererType == FlashTargets.SpriteRenderer)
            mSpriteRenderer = GetComponent<SpriteRenderer>();
        else
            mRenderer = GetComponent<MeshRenderer>();

        // Find the slected renderer type
        if ((RendererType == FlashTargets.SpriteRenderer && mSpriteRenderer == null)
            || (RendererType == FlashTargets.MeshRenderer && mRenderer == null)
            || (RendererType == FlashTargets.Image && mImage == null)
            )
        {
            Debug.LogError(string.Format("The {0} component is missing on {1}!", RendererType.ToString(), name));
            return false;
        }

        //if (RendererType == FlashTargets.None)
        //    Debug.LogError(string.Format("Flash target not set"));

        return true;
    }

    private bool bInit = false;

    void OnEnable()
    {
        bInit = Validate();

        // When re-enabling the object we check to see if there is a cached colour, if there is restore it
        // This is done incase the object is disabled while the coroutine is running
        if (bCached == false && bInit == true)
            if (RendererType == FlashTargets.SpriteRenderer)
                mColourCached = mSpriteRenderer.color;
            else if (RendererType == FlashTargets.MeshRenderer)
                mColourCached = mRenderer.material.GetColor(ColourName);
            else if (RendererType == FlashTargets.Image)
                mColourCached = mImage.color;
            else if (RendererType == FlashTargets.None)
                mColourCached = Color.white;
        else
            SetColour(CurrentColour);

        bCached = true;
    }

    /// <summary>
    /// Initializes the flash object
    /// </summary>
    public void Init()
    {
        OnEnable();
    }

    /// <summary>
    /// Starts the flash effect
    /// </summary>
    public void StartFlash()
    {
        if (bInit == true) 
            StartCoroutine(FlashAnimate());
    }

    /// <summary>
    /// Sets the flash colour
    /// </summary>
    /// <param name="_colour">Colour value</param>
    private void SetColour(Color _colour)
    {
        if (RendererType == FlashTargets.SpriteRenderer)
            mSpriteRenderer.color = _colour;
        else if (RendererType == FlashTargets.MeshRenderer)
            mRenderer.material.SetColor(ColourName, _colour);
        else if (RendererType == FlashTargets.Image)
            mImage.color = _colour;
    }

    /// <summary>
    /// Flash animation coroutine
    /// </summary>
    IEnumerator FlashAnimate()
    {
        Echo = Echo < 1 ? 1 : Echo; // Sanity check echo value

        float Movement = 0;
        float echoing = 1;
        float EchoChange = 1 / Echo;

        // If the echo is above the threshold...keep flashing
        while (echoing > 0.2f)
        {
            Movement = 0;
            while (Movement < Attack * echoing) // Get brighter...
            {
                yield return new WaitForFixedUpdate();

                Movement += Time.fixedDeltaTime;
                CurrentColour = Color.Lerp(mColourCached, FlashColour, Movement / (Attack * echoing));

                SetColour(CurrentColour);
                
            }

            Movement = 0;
            while (Movement < Sustain * echoing) // Stay at thi brightness...
            {
                yield return new WaitForFixedUpdate();
                Movement += Time.fixedDeltaTime;
            }

            Movement = 0;
            while (Movement < Drop * echoing) // Return to original colour...
            {
                yield return new WaitForFixedUpdate();
                Movement += Time.fixedDeltaTime;
                CurrentColour = Color.Lerp(FlashColour, mColourCached, Movement / (Drop * echoing));

                SetColour(CurrentColour);
            }
            echoing -= EchoChange; // Update the echo value and repeat

        }

        //Debug.LogError("Flash complete event" + name);
        if (FlashCompleteEvent != null)
            FlashCompleteEvent(this, System.EventArgs.Empty);


    }

    /// <summary>
    /// Set to true to test the flash effect
    /// </summary>
    [Tooltip("Set to true to test the flash effect")]
    public bool test = false;

    void Update()
    {
        if (test == true)
        {
            test = false;
            StartCoroutine(FlashAnimate());
        }
    }


}
