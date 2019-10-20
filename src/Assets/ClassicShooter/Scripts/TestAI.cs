using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class TestAI : MonoBehaviour {

	private AITarget mAITarget;
    public Transform BulletPrefab;
    
    public float MaxSpeed;
    public float MaxFireRange = 20;
    public float RotationSpeed;
    public float RotationThreshold;
    public float AccelerationForce;
    Rigidbody2D rb2d;

    private Shooter mShooter;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        mShooter = gameObject.GetComponent<Shooter>();
        //mShooter.BulletPrefab = BulletPrefab;
        mAITarget = gameObject.GetComponent<AITarget>();
    }

    void Start()
    {
        if (mAITarget == null)
            Debug.LogError("Player not found");   
    }

    // Returns the rotation value of an object turning from current angle to target angle at rotational speed, if the difference 
    // of the current angle is greater than the threshold
    public static float GetAngleTurnToFace(float _angleDegTarget, float _angleDegCurrent, float _angleDegThreshold, float _rotationDegPerSecond)
    {
        //print = false;
        //if (time <= 0)
        //{
        //    print = true;
        //    time = printTime;
        //}
        //time -= Time.deltaTime;

        //_angleDegCurrent += 90;
        float rotation = Time.deltaTime * _rotationDegPerSecond;

        //Print("Angle Target = " + _angleDegTarget + " Current = " + _angleDegCurrent);
        _angleDegTarget += (-90 + 180);
        _angleDegTarget += 360;
        _angleDegTarget = _angleDegTarget % 360;
        if (_angleDegTarget > 360) { _angleDegTarget -= 360; }
        float diff = Mathf.Abs(_angleDegCurrent - _angleDegTarget);

        float rv = 0;
        if (_angleDegCurrent == _angleDegTarget)
        {
            rv = 0;
        }
        else if (_angleDegCurrent > _angleDegTarget && diff > _angleDegThreshold)
        {
            if (diff < rotation)
            {
                rotation = diff;
            }

            if (diff > 180)
            {
                rotation = -rotation;
            }
            rv = -rotation;
        }
        else if (_angleDegCurrent < _angleDegTarget && diff > _angleDegThreshold)
        {
            if (diff < rotation)
            {
                rotation = diff;
            }

            if (diff > 180)
            {
                rotation = -rotation;
            }
            rv = rotation;
        }
        //Print("2 Angle Target = " + _angleDegTarget + " Current = " + _angleDegCurrent + " rotation = " + rotation);
        return rv;
    }

    public float MinSitRange = 3;
    public float MaxSitRange = 6;

    void FixedUpdate()
    {
        if (mAITarget == null || mAITarget.HasTarget() == false)
            return;

        GoToPoint(mAITarget.Target.transform.position, MaxSitRange, MinSitRange);
        FireIfFacing();
    }
    float GetAngleBetweenUs(Vector3 _pos, Vector3 _pos2)
    {
        Vector2 Difference = (Vector2)_pos - (Vector2)_pos2;

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;

        //angle = 180 - 90;// +360;
        angle += 360;
        angle = angle % 360;
        return angle;
    }

    public float cooldown = 1f;
    float time = 0;
    public float Warmup = 2;
    private float warmTime = 0;
    private bool StartFiring = false;
    public Transform WarmupPrefab;
    private Transform warmEffect;
    void FireIfFacing()
    {
        if (mAITarget.HasTarget() == false)
            return;

        float angleFacing = transform.eulerAngles.z - 90;
        angleFacing += 360;
        angleFacing = angleFacing % 360;

        float distance= (transform.position - mAITarget.Target.position).magnitude;
        if (distance > MaxFireRange)
        {
            return;
        }
            

        float angle = GetAngleBetweenUs(transform.position, mAITarget.Target.position);

        float diff = Mathf.Abs(angleFacing - angle);
        
        time -= Time.deltaTime;
        //Mortar m;
        if (BulletPrefab.name == "PlasmaBolt")
        {
            PlasmaBolt(diff);
        }
        else if (diff < 10 && time < 0)
        {
                mShooter.Fire(BulletPrefab);
                time = cooldown;
        }

        

    }

    public bool DebugOn = false;
    private string lastMessage;
    private float throttleTime;
    public void Print(string _text)
    {
        if (DebugOn)
        {
            if (lastMessage == _text)
            {
                throttleTime += Time.fixedDeltaTime;
                if (throttleTime > 1)
                {
                    Debug.Log(_text);
                    lastMessage = _text;
                    throttleTime = 0;
                }

            }
            else
            {
                Debug.Log(_text);
                lastMessage = _text;
                throttleTime = 0;
            }
            
        }
    }

    void PlasmaBolt(float diff)
    {
        if (StartFiring == false && diff <= 10)
        {
            StartFiring = true;
            Transform t = (Transform)Instantiate(WarmupPrefab, mShooter.FirePorts[0].position, transform.rotation);
            t.parent = transform;
            StartCoroutine(PlasmaBoltCancel());
        }
        else if (StartFiring == true && diff <= 10)
        {
            
            warmTime += Time.fixedDeltaTime;
            if (warmTime > Warmup)
            {
                mShooter.Fire(BulletPrefab);
                StartFiring = false;
                warmTime = 0;
            }
                
        }
        else if (StartFiring == true && diff > 10)
        {
            warmTime = 0;
            Destroy(warmEffect);
            StartFiring = false;
        }
        else
        {
            StartFiring = false;
            warmTime = 0;
        }

    }

    IEnumerator PlasmaBoltCancel()
    {
        yield return new WaitForSeconds(Warmup+0.03f);
        StartFiring = false;
        warmTime = 0;
    }

    void GoToPoint(Vector2 Point, float _maxRange, float _minRange)
    {
        // Rotation part
        Vector2 Difference = (Vector2)transform.position - Point;
        float speedMod = 1;

        float angle = Mathf.Atan2(Difference.y, Difference.x) * 57.2f;
            
        float CurrentSpeed = rb2d.velocity.magnitude;
        float currentRotation = Utils.Utilities.LerpPart(0, MaxSpeed, 0.3f, RotationSpeed, CurrentSpeed);

        float rotation = GetAngleTurnToFace(angle, transform.eulerAngles.z, RotationThreshold, currentRotation);

        transform.Rotate(0, 0, rotation);
        float mag = Difference.magnitude;
        if (mag > _maxRange)
        {
            //Debug.Log("Going forwards");
            Vector2 direction = transform.up.normalized;
            direction *= AccelerationForce * speedMod;
            rb2d.velocity = direction;
        }
        else if (mag < _minRange)
        {
            Vector2 direction = transform.up.normalized * -0.5f; // Go backwards slowly
            direction *= AccelerationForce;
            rb2d.velocity = direction;
        }
        
        
        


        



    }
}
