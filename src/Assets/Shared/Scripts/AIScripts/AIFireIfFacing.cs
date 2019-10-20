using UnityEngine;
using System.Collections;

/// <summary>
/// Triggers the shooter object to fire with selected weapon if facing the target of AITarget
/// Combimed with other AI scripts this allows you to create simple enemies
/// </summary>
[RequireComponent(typeof(AITarget))]
[RequireComponent(typeof(Shooter))]
public class AIFireIfFacing : MonoBehaviour {

    private AITarget    mTargeter;
    protected Shooter     mShooter;

    /// <summary>
    /// Delay between shots
    /// </summary>
    public float        FireDelay; // Delay between shots

    /// <summary>
    /// Maximum range which the unit will fire from. 0 is infinite
    /// </summary>
    [Tooltip("Maximum range which the unit will fire from. 0 is infinite")]
    public float        MaxRange;

    /// <summary>
    /// Selected weapon
    /// </summary>
    public Transform    BulletPrefab;

    public delegate void FiredShot(Bullet[] _shots);
    public event FiredShot FiredShotEvent;

    void Start()
    {
        mTargeter = gameObject.GetComponent<AITarget>();
        mShooter = gameObject.GetComponent<Shooter>();
    }

    /// <summary>
    /// Calculate the angle between two position
    /// </summary>
    /// <param name="_pos">First object position</param>
    /// <param name="_pos2">Second object position</param>
    /// <returns>Angle in degrees</returns>
    float GetAngleBetweenUs(Vector3 _pos, Vector3 _pos2)
    {
        Vector2 Difference = (Vector2)_pos - (Vector2)_pos2;
        
        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;
        
        angle += 360;
        angle = angle % 360;
        return angle;
    }

    float mTimePassed = 0;

    public bool debugIsFacing = false;

    /// <summary>
    /// Update tick for detecting whether this object is facing the target object
    /// </summary>
    public void UpdateIsFacing()
    {
        float angleFacing = transform.eulerAngles.z - 90; // Sprites are at 90 degrees to "UP" so we add this fudge here
        angleFacing += 360;                 // Deals with wrap around
        angleFacing = angleFacing % 360;    // Deals with wrap around

        float angle = GetAngleBetweenUs(transform.position, mTargeter.Target.transform.position);

        float diff = Mathf.Abs(angleFacing - angle);

        if (diff < 10)
        {
            debugIsFacing = true;
        }
        else
            debugIsFacing = false;

    }

    void FixedUpdate()
    {
        if (mTargeter is null)
            return;
        if (mTargeter.HasTarget() == false)
            return;

        mTimePassed += Time.fixedDeltaTime;

        UpdateIsFacing();

        if (MaxRange > 0)
        {
            float distance = (transform.position - mTargeter.Target.position).magnitude;

            if (distance > MaxRange)
                return;
        }

        if (mTimePassed < FireDelay)
            return;

        float angleFacing = transform.eulerAngles.z - 90;
        angleFacing += 360;                 // Deals with wrap around
        angleFacing = angleFacing % 360;    // Deals with wrap around

        float angle = GetAngleBetweenUs(transform.position, mTargeter.Target.transform.position);

        float diff = Mathf.Abs(angleFacing - angle);

        // Angle between us is less than this fudge value
        if (diff < 10)
        {
            Fire();
        }
    } // fixed update

    /// <summary>
    /// Tells the shooter to fire, adds the target info to AttackData which is passed to the bullet (used for homing attacks etc)
    /// </summary>
    public virtual void Fire()
    {
        
        // Supply the target information
        AttackData myTargets = new AttackData();
        myTargets.AddTarget(mTargeter.Target);

        if (!mShooter.Fire(BulletPrefab, myTargets))
            return; // If no shot is fired return


        // Get the last shot and store it in a list.
        Bullet b = mShooter.GetLastBullet();

        Bullet[] blist = new Bullet[1];
        blist[0] = b;

        mTimePassed = 0;

        // Trigger events that are waiting on this firing
        if (FiredShotEvent != null)
            FiredShotEvent(blist);

    } // fire

} // class 
