using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a damage dealer that deals damage as an expanding 2d wave.
/// Contains hackey part that also triggers shake on player - this part will be moved to a seperate class in future versions
/// </summary>
[RequireComponent(typeof(DamageDealer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Shockwave : MonoBehaviour {

    /// <summary>
    /// Adds camera shake
    /// </summary>
    [Tooltip("Adds camera shake")]
    public float MaxShakeDistance = 3;

    /// <summary>
    /// Rate of shockwave expansion
    /// </summary>
    [Tooltip("Rate of shockwave expansion")]
    public float CollisionCircleExpandTime = 1.5f;

    private CircleCollider2D mCollider;

    private float mMaxRadius;

    void OnValidate()
    {
        MaxShakeDistance = Mathf.Max(0, MaxShakeDistance);
        CollisionCircleExpandTime = Mathf.Max(0, CollisionCircleExpandTime);
    }

    void Start()
    {
        mCollider = gameObject.GetComponent<CircleCollider2D>();

        mMaxRadius = mCollider.radius;
        mCollider.radius = 0.01f;

        /// Beyond this point is player screen shaking stuff.
        /// Will be move to a seperate class in future versions
        
        Player p = null;
        GameObject gp = GameObject.FindGameObjectWithTag("Player");        

        if (gp != null)
            p = gp.GetComponent<Player>();

        if (p == null)
            return;

        // Only concerned with shaking the screen
        Vector3 diff = p.transform.position - transform.position;
        float distance = diff.magnitude;
        float Intensity = (1 - Mathf.Clamp(distance / MaxShakeDistance, 0, 1)) * 8;

        //Debug.Log("Shaking screen " + Intensity + " dist = " + distance);
        if (Intensity > 0)
            p.ShakeScreen(Intensity, 0.4f, true);

        
    }

    float countToTwo = 0;

    void FixedUpdate()
    {
        if (mCollider.radius < mMaxRadius)
        {
            mCollider.radius += (Time.fixedDeltaTime / CollisionCircleExpandTime) * mMaxRadius;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider.gameObject.tag == "Player")
        {
            Rigidbody2D p = _collider.gameObject.GetComponent<Rigidbody2D>();
            //p.AddForceAtPosition(new Vector2(Random.Range(-300,300), Random.Range(-300,300)), transform.position);
            float strength = 10;
            Vector2 force = p.transform.position - transform.position;
            force = force.normalized * strength;

            p.AddForce(force, ForceMode2D.Impulse);
            
        }

    }
}
