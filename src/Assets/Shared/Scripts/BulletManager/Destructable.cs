using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

/// <summary>
/// Event args containing the damaged object and the damage dealer. Sent with the DamageTaken event
/// </summary>
public class DamageEventArgs : EventArgs
{
    /// <summary>
    /// Constructor for DamageEventArgs object
    /// </summary>
    /// <param name="_dest">Destructable that was damaged</param>
    /// <param name="_dealer">DamageDealer that the damage came from</param>
    public DamageEventArgs(Destructable _dest, DamageDealer _dealer)
    {
        DamagedObject = _dest;
        DamageDealer  = _dealer;
    }

    /// <summary>
    /// Destructable that was damaged
    /// </summary>
    public Destructable DamagedObject;

    /// <summary>
    /// DamageDealer that the damage came from
    /// </summary>
    public DamageDealer DamageDealer;
}

// 
/// <summary>
/// A standard component that can be added to quickly make something "killable". Handles damage dealt from DamageDealer objects
/// Requires a HitConfig component to operate. Destructable is like RigidBody where it needs a collider to know how to behave
/// </summary>
public class Destructable : MonoBehaviour 
{
    private HitConfigBase mConfig = null;

    /// <summary>
    /// Currently used hit config. (This is like the collider to a rigidbody)
    /// </summary>
    public HitConfigBase Config
    {
        get { return mConfig; }
        private set { mConfig = value; }
    }

    /// <summary>
    /// Effect that is spawned when this destructable is destroyed
    /// </summary>
    [Tooltip("Effect that is spawned when this destructable is destroyed")]
    public Transform DestroyedEffectPrefab;

    public delegate void DamageTaken(System.EventArgs _damageEventArgs);

    /// <summary>
    /// Triggered whenever the destructable takes damage. Passes DamageEventArgs
    /// </summary>
    public event DamageTaken DamageTakenEvent;

    public delegate void Destroyed(System.EventArgs _damageEventArgs);

    /// <summary>
    /// Triggered when the destructable is killed
    /// </summary>
    public event Destroyed DeadEvent;

    /// <summary>
    /// Destroys the game object when it is killed. Set to false if you want it to stick around for animations etc
    /// </summary>
    [Tooltip("Destroys the game object when it is killed. Set to false if you want it to stick around for animations etc")]
    public bool DestroyOnDead = true;

    protected bool bInvulnerable;

    /// <summary>
    /// Sets/gets whether the unit is currently immune to damage
    /// </summary>
    public bool Invulnerable
    {
        get { return bInvulnerable; }
        set { bInvulnerable = value; }
    }

    /// <summary>
    /// Creates default values for this component
    /// </summary>
    public void Reset()
    {
        Config = GetComponent<HitConfigBase>();
        if (Config == null)
            Config = gameObject.AddComponent<SimpleHitpoints>();
    }

    /// <summary>
    /// Sets the active some component for Destrucable component, only do this in editor
    /// </summary>
    /// <param name="_config">Config to assign to this Destructable</param>
    public void SetConfig(HitConfigBase _config)
    {
        if (Application.isEditor == false)
        {
            Debug.LogError("This is an editor only function");
            return;
        }
        
        Config = _config;
        HitConfigBase[] configs = GetComponents<HitConfigBase>();

        // Removes previous components
        for (int i = 0; i < configs.Length; i++)
        {
            if (configs[i] != Config)
                DestroyImmediate(configs[i]);
        }
        
    }

    void OnEnable()
    {
        Config = GetComponent<HitConfigBase>();
    }

    /// <summary>
    /// Validates the Destructable
    /// </summary>
    /// <returns>bool</returns>
    public virtual bool ValidateConfig()
    {

        Config = GetComponent<HitConfigBase>();

        if (Application.isEditor == false)
        {
            Debug.LogError("This is an editor only function");
            return false;
        }
        if (Config == null)
        {
            Config = gameObject.AddComponent<SimpleHitpoints>();
        }

        return false;
    }

    void OnValidate()
    {
        ValidateConfig();
    }

    void OnCollisionEnter2D(Collision2D _collision)
    {
        if (bInvulnerable)
            return;

        DamageDealer d =  _collision.collider.gameObject.GetComponent<DamageDealer>();
        if (d != null)
            RecieveDamage(d);
    }

    void OnTriggerEnter2D(Collider2D _collider)
    {
        if (bInvulnerable)
            return;

        DamageDealer d = _collider.gameObject.GetComponent<DamageDealer>();
        if (d != null)
            RecieveDamage(d);
    }

    /// <summary>
    /// Kills this object immidietly, ignores invulnerability 
    /// </summary>
    /// <param name="_d">Damage dealer to set as the killer (can be null)</param>
    public void Kill(DamageDealer _d)
    {
        Dead(_d);
    }

    /// <summary>
    /// Recieve damage from the specified damage dealer
    /// </summary>
    /// <param name="_d"></param>
    void RecieveDamage(DamageDealer _d)
    {
        if (Config.RecieveDamage(_d) && DamageTakenEvent != null)
                DamageTakenEvent(new DamageEventArgs(this, _d));
        
        if (Config.IsDead)
            Dead(_d);

        if (_d.DestroyOnHit)
        {
            Destructable dest = _d.gameObject.GetComponent<Destructable>();
            if (dest != null)
            {
                dest.Dead(null);        // This is not an intended use case 
                Debug.LogWarning(string.Format("Destructable object {0} is being destroyed for dealing damage to {1}", dest.name, this.name));
            }
            else
            {
                Destroy(_d.gameObject, 0.01f);
            }
        }
    }

    /// <summary>
    /// Unit is dead. Do cleanup and trigger events.
    /// </summary>
    /// <param name="_d">Damage dealer that killed this unit</param>
    void Dead(DamageDealer _d)
    {
        if (DestroyedEffectPrefab != null)
            Instantiate(DestroyedEffectPrefab, transform.position, Quaternion.identity);

        if (DeadEvent != null)
        {
            DeadEvent(new DamageEventArgs(this, _d));
        }

        if (DestroyOnDead == true)
            Destroy(gameObject, 0.01f); // Leave a tiny delay so that all the events that trigger in the next frame have access to this objects data.
    }

}
