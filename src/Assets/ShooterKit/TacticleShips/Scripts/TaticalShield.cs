using UnityEngine;
using System.Collections;

/// <summary>
/// This shield class is purely cosmetic. Provides a transparent layer that flashes when damage is recieved
/// </summary>
public class TaticalShield : MonoBehaviour {

    /// <summary>
    /// Object being damaged
    /// </summary>
    public Destructable Ship;

    private Flash mFlash;
    private SpriteRenderer mSprite;

    bool mInitialized = false;

    /// <summary>
    /// Validates the shield 
    /// </summary>
    /// <returns></returns>
    bool Validate()
    {
        bool bValid = true; ;

        mFlash = GetComponent<Flash>();
        if (mFlash == null)
        {
            Debug.LogError("Flash component is missing");
            bValid = false;
        }
        mSprite = GetComponent<SpriteRenderer>();
        if (mSprite == null)
        {
            Debug.LogError("Sprite component is missing");
            bValid = false;
        }

        if (Ship == null)
        {
            Debug.LogError("Ship has not been assigned");
            bValid = false;
        }
        return bValid;
    }

    void OnValidate()
    {
        Validate();
    }

    void OnEnable()
    {
        mInitialized = Validate();

        if (!mInitialized)
            return;

        Ship.DamageTakenEvent += new Destructable.DamageTaken(Ship_DamageTakenEvent);

    }

    /// <summary>
    /// Handler for damage taken event
    /// </summary>
    /// <param name="_damageEventArgs"></param>
    void Ship_DamageTakenEvent(System.EventArgs _damageEventArgs)
    {
        mFlash.StartFlash();

        CheckShieldState();
    }

    /// <summary>
    /// Removes this sprite if the shield is taken down (read from the hit config
    /// </summary>
    void CheckShieldState()
    {
        TacticalHitConfig thc = Ship.Config as TacticalHitConfig;
        if (thc.Shield <= 0)
            mSprite.enabled = false;
        else
            mSprite.enabled = true;
    }

}
