using UnityEngine;
using System.Collections;

/// <summary>
/// Life sprite for the HUD UI. Flashes when a life is lost and then becomes transparent.
/// </summary>
[RequireComponent(typeof(Flash))]
public class LifeSprite : MonoBehaviour {

    private Flash mFlash;
    void Awake()
    {
        OnEnable();
    }

    void OnEnable()
    {
        if (mFlash != null)
            return;

        mFlash = (Flash)gameObject.GetComponent<Flash>();
        if (mFlash == null)
            Debug.LogError("Flash component is missing");

        mFlash.FlashCompleteEvent += new System.EventHandler(mFlash_FlashCompleteEvent);
    }

    void OnDisable()
    {
        mFlash.FlashCompleteEvent -= mFlash_FlashCompleteEvent;
        mFlash = null;
    }

    void mFlash_FlashCompleteEvent(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    public void EnableLifeSprite()
    {
        gameObject.SetActive(true);
        
    }
    public void DisableLifeSprite()
    {
        mFlash.StartFlash();
    }
    
}
