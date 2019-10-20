using UnityEngine;
using System.Collections;
using Utils;

public class AIGoTo : MonoBehaviour {

    //public Transform Target;

    protected AITarget mTarget;


    public float RotationSpeed;
    public float RotationThreshold;

    /// <summary>
    /// How quickly the AI unit accelerates towards the target
    /// </summary>
    [Tooltip("How quickly the AI unit accelerates towards the target")]
    public float Acceleration; // Meters^-2s

    /// <summary>
    /// Speed at which the AI Unit travels
    /// </summary>
    [Tooltip("Speed at which the AI Unit travels")]
    public float Speed;

    /// <summary>
    /// Max distance the unit is happy to sit at on arrival
    /// </summary>
    [Tooltip("Max distance the unit is happy to sit at on arrival")]
    public float ArrivalMaxDistance;

    // TODO: Move the min distance code from TestAI script
    /// <summary>
    /// Minimum distance the unit is happy to sit at before moving away from the target location
    /// </summary>
    [Tooltip("Minimum distance the unit is happy to sit at before moving away from the target location")]
    public float ArrivalMinDistance;

    private Rigidbody2D rb2d;

    private bool mArrived = false;

    /// <summary>
    /// Returns whether the unit is in arrived state or not
    /// </summary>
    public bool Arrived
    {
        get { return mArrived; }
        private set { mArrived = value; }
    }

    protected void OnEnable()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        mTarget = gameObject.GetComponent<AITarget>();
    }

    void GoToPoint(Vector2 Point, float _maxRange, float _minRange)
    {
        // Rotation part
        Vector2 Difference = (Vector2)transform.position - Point;
        float speedMod = 1;

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;

        float CurrentSpeed = rb2d.velocity.magnitude;
        float currentRotation = Utils.Utilities.LerpPart(0, Speed, 0.3f, RotationSpeed, CurrentSpeed);

        float rotation = Utils.Utilities.GetAngleTurnToFace(angle, transform.eulerAngles.z, RotationThreshold, currentRotation);

        transform.Rotate(0, 0, rotation);
        float mag = Difference.magnitude;
        if (mag > _maxRange)
        {
            //Debug.Log("Going forwards");
            Vector2 direction = transform.up.normalized;
            direction *= Acceleration * speedMod;
            rb2d.velocity = direction;
        }
        else if (mag < _minRange)
        {
            Vector2 direction = transform.up.normalized * -0.5f; // Go backwards slowly
            direction *= Acceleration;
            rb2d.velocity = direction;
        }
    }
    protected void FixedUpdate()
    {
        if (mTarget.HasTarget() == false)
            return;

        GoToPoint(mTarget.Target.transform.position, ArrivalMaxDistance, ArrivalMinDistance);
        return;
        
        Vector2 Difference = (Vector2)transform.position - (Vector2)mTarget.Target.position;
        float distance = Difference.magnitude;
        if (distance < ArrivalMaxDistance)
            Arrived = true;
        else
            Arrived = false;

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;

        float rotation = Utilities.GetAngleTurnToFace(angle, transform.eulerAngles.z, RotationThreshold, RotationSpeed);

        transform.Rotate(0, 0, rotation);

        if (Arrived)
            return;

        Vector2 applied = transform.up * Acceleration;

        Vector2 velocity = rb2d.velocity;
        velocity += applied * Time.deltaTime;

        

        if (velocity.magnitude > Speed)
            velocity = Speed * velocity.normalized;

        
        if (distance < 1)
        {
            velocity *= Difference.magnitude;
        }
        
        rb2d.velocity = velocity;

    }

    /// <summary>
    /// Sets the AI unit to arrived state
    /// </summary>
    private void SetArrived()
    {
        Arrived = true;
        
    }
}
