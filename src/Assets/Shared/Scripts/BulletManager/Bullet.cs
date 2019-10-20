using UnityEngine;
using System.Collections;

/// <summary>
/// Provides options for how the bullet deals with collisions
/// </summary>
public enum BulletCollisionSettings
{
    ParentLayer,
    UserDefinedLayer,
    TargetListOnly,
    NoCollisions,
}

/// <summary>
/// Basic bullet class
/// Handles velocity, spawn layer and contains attack data from the firer
/// </summary>
[RequireComponent(typeof(DamageDealer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    /// <summary>
    /// Speed at which the bullet travels (velocity of the firing unity is added too)
    /// </summary>
    [Tooltip("Speed at which the bullet travels (velocity of the firing unity is added too)")]
    public float Speed;

    /// <summary>
    /// Destroy object once it has collided with an object
    /// </summary>
    [Tooltip("Destroy object once it has collided with an object")]
    public bool DestroyOnHit = true;

    /// <summary>
    /// Hit effect to spawn when the bullet collides with an object
    /// </summary>
    [Tooltip("Hit effect to spawn when the bullet collides with an object")]
    public Transform BulletHitEffectPrefab;

    /// <summary>
    /// Data that is passed from the shooting object to the bullet, used to convey special information like powerups bonuses etc
    /// </summary>
    [Tooltip("Data that is passed from the shooting object to the bullet, used to convey special information like powerups bonuses etc")]
    public AttackData AttackData;

    /// <summary>
    /// Use this to choose how bullet collisions are handled. ParentLayer is the simplest and easiest.
    /// </summary>
    [Tooltip("Use this to choose how bullet collisions are handled. ParentLayer is the simplest and easiest.")]
    public BulletCollisionSettings BulletCollisionSetting;

    /// <summary>
    /// Only visible when BulletCollisionSetting is set to "User Defined Layer"
    /// Layer to spawn the bullet into
    /// </summary>
    [Tooltip("Layer to spawn the bullet into")]
    public SingleUnityLayer SpawnLayer;

    protected Transform mOwner;
    protected Rigidbody2D rb2d;

    /// <summary>
    /// The transform of the unit that fired this bullet 
    /// </summary>
    public Transform Owner
    {
        get { return mOwner; }
        set { mOwner = value; }
        
    }

    public delegate void BulletDestroyed(Bullet _b);
    public event BulletDestroyed BulletDestroyedEvent;

    protected bool mInitialized = false;

    /// <summary>
    /// Validation function for when the bullet starts up
    /// </summary>
    /// <returns>bool</returns>
    bool Validate()
    {
        bool valid = true;

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == false)
        {
            Debug.LogError("Rigidbody2d is missing, add component in editor");
            valid = false;
        }

        return valid;
    }

    // This is used instead of OnEnable, because we want this to happen after the creating object (factory) has finished setting 
    // the varaibles
    protected virtual void Start()
    {
        mInitialized = Validate();

        if (!mInitialized)
            return;

        rb2d.velocity += Speed * (Vector2)transform.up;

        if (rb2d.velocity.magnitude < Speed) // If the velocity is negative relative to the direction of travel set the velocity to the minimum speed
            rb2d.velocity = rb2d.velocity.normalized * Speed;
    }

    protected void SetVelocity()
    {
        
    }

    private void OnDestroy()
    {
        if (BulletDestroyedEvent != null)
            BulletDestroyedEvent(this);
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        SpawnHitEffect(_collision.contacts[0].point);
        if (DestroyOnHit == true)
            Destroy(gameObject);
    }

    /// <summary>
    /// Creates the hit effect prefab when a collision occurs
    /// </summary>
    /// <param name="_pos">Position to spawn the effect at</param>
    /// <returns>Returns transform of hit effect</returns>
    protected Transform SpawnHitEffect(Vector2 _pos)
    {
        Transform t = null;
        if (BulletHitEffectPrefab != null)
            t = (Transform)Instantiate(BulletHitEffectPrefab, _pos, this.transform.rotation);
        return t;
    }
}
