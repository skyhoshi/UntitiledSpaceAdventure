using UnityEngine;
using System.Collections;

/* Causes the object to flash when hit (requires shader with "_TintColor") */
[RequireComponent(typeof(Flash))]
public class FlashOnHit : MonoBehaviour {

    public Renderer         mRenderer;          //Set which renderer to change
    private SpriteRenderer  mSpriteRenderer;
    public Color            FlashColour;
    
    public float Attack;    // How quickly the object lights up        
    public float Sustain;   // How long it stays lit
    public float Echo;      // Number of repeats
    public float Drop;      // How quickly it returns to its original colour

    public string ColourName = "_TintColor";

    public bool useSprite = false;
    // Private cached vars
    private Color       mColourCached;
    private Color       CurrentColour;
    private bool        bCached = false;

    private Flash mFlash; 
    void OnEnable()
    {
        mFlash = gameObject.GetComponent<Flash>();
        if (mFlash == null)
        {
            Debug.LogError("Flash component is missing on " + name);
            return;
        }
        return;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        // When re-enabling the object we check to see if there is a cached colour, if there is restore it
        // This is done incase the object is disabled while the coroutine is running
        if (bCached == false)
            if (useSprite && mSpriteRenderer != null)
                mColourCached = mSpriteRenderer.color;
            else
                mColourCached = mRenderer.material.GetColor(ColourName);
        else
            if (useSprite && mSpriteRenderer != null)
                mSpriteRenderer.color = mColourCached;
            else
                mRenderer.material.SetColor(ColourName, mColourCached);

        bCached = true;
    }

    // Trigger the flash
    void OnCollisionEnter2D(Collision2D _collision)
    {
        mFlash?.StartFlash();
        //StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Echo = Echo < 1 ? 1 : Echo; // Sanity check echo value

        float Movement      = 0;
        float echoing       = 1;
        float EchoChange    = 1 / Echo;

        // If the echo is above the threshold...keep flashing
        while (echoing > 0.2f)
        {
            Movement = 0;
            while (Movement < Attack * echoing) // Get brighter...
            {
                yield return new WaitForFixedUpdate();

                Movement += Time.fixedDeltaTime;
                CurrentColour = Color.Lerp(mColourCached, FlashColour, Movement / (Attack* echoing));

                if (useSprite && mSpriteRenderer != null)
                    mSpriteRenderer.color = CurrentColour;
                else
                    mRenderer.material.SetColor(ColourName, CurrentColour);
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

                if (useSprite && mSpriteRenderer != null)
                    mSpriteRenderer.color = CurrentColour;
                else
                    mRenderer.material.SetColor(ColourName, CurrentColour);
            }
            echoing -= EchoChange; // Update the echo value and repeat
            
        }
        
        
    }


}
