using UnityEngine;
using System.Collections;
using Utils;

/// <summary>
/// Extends bullet to add the tracking behavior
/// </summary>
public class HomingBullet : Bullet {

    /// <summary>
    /// Object to follow
    /// </summary>
    [Tooltip("Object to follow")]
    public Transform Target;

    protected void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity += Speed * (Vector2)transform.up;

        if (rb2d.velocity.magnitude < Speed) // If the velocity is negative relative to the direction of travel set the velocity to the minimum speed
            rb2d.velocity = rb2d.velocity.normalized * Speed;
        mRandomOffset = new Vector2(Random.RandomRange(-Offset, Offset), Random.RandomRange(-Offset, Offset));

        HomingAttackData hd = AttackData as HomingAttackData;
        if (hd != null)
            Target = hd.Target;
    }

    /// <summary>
    /// Optional random offset so not all projectiles hit exactly the same point on the target object. This is for visual variety.
    /// </summary>
    [Tooltip("Optional random offset so not all projectiles hit exactly the same point on the target object. This is for visual variety.")]
    public float Offset = 1.51f;

    /// <summary>
    /// Cache of offset if there is one.
    /// </summary>
    private Vector2 mRandomOffset;

    /// <summary>
    /// Rotation speed of the bullet
    /// </summary>
    [Tooltip("Rotation speed of the bullet")]
    public float RotationSpeed = 60f;

    /// <summary>
    /// Replaces the Update of bullet to add tracking behavior in it's steering section
    /// </summary>
    void Update()
    {
        if (Target == null)
            return;


        Vector2 Difference = (Vector2)transform.position - ((Vector2)Target.position + mRandomOffset);

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;

        float RotationThreshold = 1;
        float rotation = Utilities.GetAngleTurnToFace(angle, transform.eulerAngles.z, RotationThreshold, RotationSpeed);

        transform.Rotate(0, 0, rotation);
        vel = rb2d.velocity;

        vel += (Vector2)transform.up * Speed * Speed * Time.deltaTime;

        if (vel.magnitude > Speed)
            vel = Speed * vel.normalized;

        rb2d.velocity = vel;
        
    }
    public Vector2 vel;
    
}
