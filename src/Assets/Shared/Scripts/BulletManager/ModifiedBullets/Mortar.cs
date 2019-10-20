using UnityEngine;
using System.Collections;
using Utils;

/// <summary>
/// Extends bullet to make an weapon that "explodes" at a specific distance from it's start. Used for shells, rockets, mines, cluster bombs etc
/// </summary>
public class Mortar : Bullet
{

    /// <summary>
    /// Point to expode at
    /// </summary>
    [Tooltip("Point to expode at")]
    public Vector2 TargetPos;

    /// <summary>
    /// Optional trail (with particle effect). Trail is detatched on death and killed when the particle system finishes it's animation
    /// </summary>
    [Tooltip("Optional trail (with particle effect). Trail is detatched on death and killed when the particle system finishes it's animation")]
    public Transform Smoke;

    /// <summary>
    /// The distance the projectile travels before exploding
    /// </summary>
    [Tooltip("The distance the projectile travels before exploding")]
    public float TravelDistance = 15; // 

    /// <summary>
    /// Cached value of velocity
    /// </summary>
    private Vector3 velocityCached;

    new void Start()
    {

        Vector2 rot = transform.up;

        TargetPos = (rot * TravelDistance) + (Vector2)transform.position; // Picks a point an arbitrary distance infront of it to explode at

        // Test script to test effectiveness
        //base.Start();
        base.Start();
        //base.WeaponSystemBulletStart();
        velocityCached = rb2d.velocity;
    }

    /// <summary>
    /// Keeps track of if this unit has exploded already. (We only want it to go off once!)
    /// </summary>
    bool bDestroyed = false;

    void OnCollisionEnter2D(Collision2D _collision)
    {
        SpawnExplodeEffect(_collision.contacts[0].point);
    }

    /// <summary>
    /// Spawns the explode effect of this bullet. For Mortar type the exploding part will likely contain damage dealers or more bullets
    /// </summary>
    /// <param name="_pos">Position to create the new objects</param>
    void SpawnExplodeEffect(Vector3 _pos)
    {
        if (bDestroyed == true)
            return;

        Transform t = SpawnHitEffect(_pos);
        if (t != null)
            t.gameObject.layer = gameObject.layer;

        // Prevent it damaging owner // This was done before I moved this mortar to the "EmptyShots" category
        if (mOwner != null)
        {
            Collider2D[] colliders = mOwner.gameObject.GetComponentsInChildren<Collider2D>();
            Shooter.IgnoreColliders(t, colliders);
        }

        if (Smoke != null)
        {   // Remove the smoke, let it's particule systems run it's course and then die.
            DetachAndDie d = Smoke.gameObject.AddComponent<DetachAndDie>();
            d.RunDetachAndDie();
        }

        if (DestroyOnHit == true)
        {
            Destroy(gameObject);
            bDestroyed = true;
        }

    }

    void FixedUpdate()
    {
        if (!mInitialized)
            return;

        Vector2 diff = TargetPos - (Vector2)transform.position;

        float distance = diff.magnitude;
        if (distance < 0.5f && bDestroyed == false)
        {
            SpawnExplodeEffect(transform.position);
            Destroy(gameObject); // Destroy this object
            return;
        }

        if (distance < 1 && distance != 0)
            rb2d.velocity = velocityCached * (distance); // Lerp the velocity down to 0 as it reaches the target, this removes oscillation.
    }

    /// <summary>
    /// For debug
    /// </summary>
    void OnGUI()
    {
        Debug.DrawLine(rb2d.position, TargetPos);
    }
}
