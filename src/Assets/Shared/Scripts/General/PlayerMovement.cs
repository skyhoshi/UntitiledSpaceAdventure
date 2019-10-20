using UnityEngine;
using System.Collections;

// TODO: Set up player profiles, controller 1 / 2, wasd, etc. 
// This needs more public settings. 
/// <summary>
/// Handles player movement for a top down 2d shooter character (imagine, boats, people, space ships etc). Uses the standard unity control system. This scheme works very nicely with an Xbox or PS controlller
/// </summary>
public partial class PlayerMovement : MonoBehaviour {

    private Camera cam;
    private Rigidbody2D rb2d;

    float accelerationTime = 0.0f;
    float accelerationSensitivity = 0.0005f; // This spreads the acceleration rate over more time, so a long hold accelerates more compared to a tap

    /// <summary>
    /// Acceleration of the player
    /// </summary>
    [Tooltip("Acceleration of the player")]
    public Vector2 Acceleration = Vector2.zero;

    /// <summary>
    /// Speed the player
    /// </summary>
    [Tooltip("Speed the player")]
    public float speed = 2;

    /// <summary>
    /// Max Speed the player
    /// </summary>
    [Tooltip("Max Speed the player")]
    public float MaxSpeed = 80;

    /// <summary>
    /// Enabled status
    /// </summary>
    [Tooltip("Enabled status")]
    public bool MovementDisabled = false;

    /// <summary>
    /// Movement status
    /// </summary>
    [Tooltip("Movement status")]
    public bool IsMoving = false;

    void OnEnable()
    {
        cam = Camera.main;
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Rotates the player to face a given direction. (Current attatched to right mouse)
    /// </summary>
    /// <param name="_direction">Position to face</param>
    /// <param name="_rotationalForce">rotation force applied</param>
    void RotateToDirection(Vector2 _direction, float _rotationalForce)
    {
        if (_direction.sqrMagnitude != 0)
        {

            float angle = Mathf.Atan2(_direction.y, _direction.x) * 57.2f;

            float currentAngle = transform.eulerAngles.z;
            float rotation2 = _rotationalForce;

            if (Input.GetKey(KeyCode.L))
            {

                rotation2 = 60;
            }
            float rotation = Time.deltaTime * rotation2;

            angle += (90 + 180);
            if (angle > 360) { angle -= 360; }
            float diff = Mathf.Abs(currentAngle - angle);

            if (currentAngle == angle)
            {
                // Do nothing
            }
            else if (currentAngle > angle && diff > 0.5f)
            {
                if (diff < rotation)
                {
                    rotation = diff;
                }

                if (diff > 180)
                {
                    rotation = -rotation;
                }
                transform.Rotate(0, 0, -rotation);
            }
            else if (currentAngle < angle && diff > 0.5f)
            {
                if (diff < rotation)
                {
                    rotation = diff;
                }

                if (diff > 180)
                {
                    rotation = -rotation;
                }
                transform.Rotate(0, 0, rotation);
            }

        }
    }

    /// <summary>
    /// Rotates to face the mouse click
    /// </summary>
    void MouseBasedRotation()
    {
        if (MovementDisabled == true)
        {
            return;
        }

        if (Input.GetMouseButton(1) == true)
        {
            if (cam == null)
            {
                //GameObject cameraloc = .camera;
                cam = Camera.main;//cameraloc.GetComponent<Camera>();
            }
            Vector3 temp = Input.mousePosition;
            temp.z = transform.position.z - cam.transform.position.z;
            Vector2 MousePoint = cam.ScreenToWorldPoint(temp);
            //Debug.Log("Direction = " + MousePoint + " Mp = " + Input.mousePosition + " trans pos " + trans.position);
            MousePoint = MousePoint - (Vector2)transform.position;
            MousePoint.Normalize();

            RotateToDirection(MousePoint, 200);

        }

    }

    /// <summary>
    /// Speed at which the player rotates
    /// </summary>
    [Tooltip("Speed at which the player rotates")]
    public float RotationSpeed = 750;

    public float debugInput;

    /// <summary>
    /// Dpad controls (joystick)
    /// </summary>
    public void DPad()
    {
        //Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float input = Input.GetAxis("Horizontal");
        float sign = Mathf.Sign(input);
        if (input != 0)
        {
            //return;
            input = Mathf.Abs(input);
            input = ((input * 10) * (input * 10) * (input * 10)) / 1000; // This changes the sensitivity to be cubic, (i.e. tiny nudge is tiny, big swing is huuuge)
            //Debug.Log("Input value = " + input);
            input *= -sign;
            //input *= -1;
            debugInput = input;
            transform.Rotate(0, 0, Time.deltaTime * RotationSpeed * input);
        }

        //RotateToDirection(direction, 200);
    }

    /// <summary>
    /// Unused
    /// </summary>
    public void OnScreenJoystick()
    {
        // Use the joystick axis.
    }

    /// <summary>
    /// Get forward movement from dpad (joystick)
    /// </summary>
    /// <returns></returns>
    float GetLinearMovementDpad()
    {
        return Input.GetAxis("Vertical");
    }

    /// <summary>
    /// Get forward movement from mouse & keyboard
    /// </summary>
    /// <returns></returns>
    float GetLinearMovementMouseKeyboard()
    {
        if (Input.GetKey(KeyCode.W) == true)
        {
            return 1;
        }
        else if (Input.GetKey(KeyCode.S) == true)
        {
            return -1;
        }
        return 0;
    }

    /// <summary>
    /// Detect linear movement
    /// </summary>
    void LinearMovement()
    {
        float direction = 0;

        direction = GetLinearMovementDpad();
        if (direction == 0)
        {
            GetLinearMovementMouseKeyboard();
        }

        float angle = gameObject.transform.rotation.eulerAngles.z;

        if (direction != 0)
        {
            accelerationTime = Mathf.Min(accelerationTime + (Time.deltaTime / accelerationSensitivity), 1.0f);

            float accelerationForFrame = accelerationTime;// *speed;
            Acceleration.x = -Mathf.Sin(angle / 57.295f) * accelerationForFrame * direction;
            Acceleration.y = Mathf.Cos(angle / 57.295f) * accelerationForFrame * direction;
            IsMoving = true;
        }
        else
        {
            accelerationTime = 0.0f;
            Acceleration.x = 0;
            Acceleration.y = 0;
            IsMoving = false;
        }




        //float Mass = 100;
        // speed = force                // Arbitrary reduction of acceleration, cus it's more fun this way
        Vector2 applied = Acceleration * speed;

        Vector2 velocity = rb2d.velocity;

        velocity += applied * Time.deltaTime;
        debugVal = applied * Time.deltaTime;

        float curSpeed = velocity.magnitude;

        if (curSpeed > MaxSpeed)
            velocity = MaxSpeed * velocity.normalized; 
        rb2d.velocity = velocity;
                
    }

    public Vector2 debugVal;
    public Vector2 GetVelocity()
    {
        return rb2d.velocity;
    }

    /// <summary>
    /// Detect rotation
    /// </summary>
    public void Rotation()
    {
        MouseBasedRotation();
        DPad();

    }

    /// <summary>
    /// Update movement
    /// </summary>
    void FixedUpdate()
    {
        Rotation();
        LinearMovement();
    }
}
