using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Follows a target (a player!) around. Has a little delay to add cinematic game effect
/// Handles Points of interest, which adjust the focus of the camera
/// Handles screen shake effects.
/// </summary>
public class CameraFollow : MonoBehaviour {
    
    /// <summary>
    /// Object to follow
    /// </summary>
    [Tooltip("Object to follow")]
    public Transform target;

    // Should be private
    private bool bShaking = false;
    public bool IsDefaultCamera = false;

    /// <summary>
    /// Constraint for how far the camera moves away from the 
    /// </summary>
    [Tooltip("Constraint for how far the camera moves away from the ")]
    public Vector2 ClippingBottomLeft;

    /// <summary>
    /// Constraint for how far the camera moves away from the 
    /// </summary>
    [Tooltip("Constraint for how far the camera moves away from the ")]
    public Vector2 ClippingTopRight;

    private static CameraFollow DefaultCamera;
    private Transform trans;
    
    void Awake()
    {
        try
        {
            if (DefaultCamera == null || IsDefaultCamera == true) // Set this cam as default if there isn't one already
                DefaultCamera = this;
            
            trans = transform;
            if (target == null)
                target = transform;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Get the current default camera
    /// </summary>
    /// <returns>Default Camera</returns>
    public static CameraFollow GetDefaultCamera()
    {
        return DefaultCamera;
    }

    /// <summary>
    /// Begins a screen shake effect for specified strength and duration
    /// </summary>
    /// <param name="_strength">Strength</param>
    /// <param name="_duration">Duration in seconds</param>
    public void StartScreenShake(float _strength, float _duration)
    {
        Shake(_strength, _duration);
       
    }

    /// <summary>
    /// Begins a screen shake effect for specified strength and duration
    /// </summary>
    /// <param name="_strength">Strength</param>
    /// <param name="_duration">Duration in seconds</param>
    /// <param name="_override">Overides existing effect</param>
    public void StartScreenShake(float _strength, float _duration, bool _override)
    {
        Shake(_strength, _duration, _override);
    }

    /// <summary>
    /// Transfer this camera to a new target
    /// </summary>
    /// <param name="_target">Transform target</param>
    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    /// <summary>
    /// Handler for camera shake effect
    /// </summary>
    void CamerShakeHandler(float _strength, float _duration)
    {
        Shake(_strength, _duration);
    }
	
    private Vector3 offset = Vector3.zero;

    /// <summary>
    /// Set camera offset from target
    /// </summary>
    /// <param name="_offset">Offset</param>
    public void SetOffset(Vector3 _offset)
    {
        offset = _offset;
    }
	
    /// <summary>
    /// Set camera offset from target
    /// </summary>
    /// <param name="_offset">Offset</param>
    public void SetOffset(Vector2 _offset)
    {
        offset = new Vector3(_offset.x, _offset.y, 0);
    }

    private Vector3 Position = Vector3.zero;

	void Update2() {
        
        //if (shakeAmt > 0) { return; }
        Position.x = target.position.x + offset.x;
        Position.y = target.position.y + offset.y;
        Position.z = gameObject.transform.position.z;
        trans.position = Position;
        
        
	}

    /// <summary>
    /// List of objects to move towards
    /// </summary>
    private List<Transform> PointsOfInterest = new List<Transform>();
    public Vector2 POIOffset = Vector2.zero;

    /// <summary>
    /// Add a new Point of Interest
    /// </summary>
    /// <param name="_obj">POI Transform</param>
    public void AddPOI(Transform _obj)
    {
        if (_obj == null) throw new System.Exception("Object is null");
        if (PointsOfInterest.Contains(_obj) == false)
        {
            PointsOfInterest.Add(_obj);
        }
    }

    /// <summary>
    /// remove Point of Interest
    /// </summary>
    /// <param name="_obj">POI Transform</param>
    public void RemovePOI(Transform _obj)
    {
        if (PointsOfInterest.Contains(_obj) == true)
        {
            PointsOfInterest.Remove(_obj);
        }
    }

    /// <summary>
    /// Allow points of interest to influence the camera position
    /// </summary>
    [Tooltip("Allow points of interest to influence the camera position")]
    public bool bPOI = true;

    /// <summary>
    /// Maximum range at which an object is considered in re-centering the camera
    /// </summary>
    public float InterestRange = 10;

    /// <summary>
    /// Evaluates where the camera's focus should be based on the POI that are currently on screen
    /// </summary>
    private void EvaluateFocus()
    {
        Vector2 temp;
        if (PointsOfInterest.Count > 0 && bPOI == true)
        {
            Vector2 TotalPosition = Vector2.zero;
            int count = 1;
            foreach (Transform t in PointsOfInterest)
            {
                // Space here to add weighting  count+=weight, TotalPosition+= value*weight;
                temp = (Vector2)t.position - (Vector2)target.position;
				Vector2 addition = (temp * Mathf.Clamp( (InterestRange - temp.magnitude) / InterestRange, 0, 1));
				TotalPosition += addition * 3;
                count++;
                //count += 3;
                
            }

            TotalPosition /= (count);
            TotalPosition.x = Mathf.Clamp(TotalPosition.x, ClippingBottomLeft.x, ClippingTopRight.x);
            TotalPosition.y = Mathf.Clamp(TotalPosition.y, ClippingBottomLeft.y, ClippingTopRight.y);

            // Lerp to the target position             
            Vector3 diff = TotalPosition - POIOffset;
            
            SmoothPOIPosition += diff * 0.15f;
            SmoothPOIPosition.z = trans.position.z;
            POIOffset = SmoothPOIPosition;

        }
    }

    /// <summary>
    /// Cache for camera position
    /// </summary>
    Vector3 SmoothPOIPosition = Vector3.zero;

    /// <summary>
    /// Main camera
    /// </summary>
    [Tooltip("Main camera")]
    public Camera mainCamera;

    /// <summary>
    /// Cached rotation
    /// </summary>
    [Tooltip("Cached rotation")]
    public Quaternion originRotation;

    /// <summary>
    /// Value for how quickly camera shake effects stop ocsilating
    /// </summary>
    [Tooltip("Cached rotation")]
    public float shake_decay;

    /// <summary>
    /// Value for how intense ocsilations on shake effects are 
    /// </summary>
    [Tooltip("Value for how intense ocsilations on shake effects are ")]
    public float shake_intensity;

    private Vector3 originPosition;

    void FixedUpdate()
    {
        try
        {
            EvaluateFocus();
            Position.x = target.position.x + offset.x + POIOffset.x;
            Position.y = target.position.y + offset.y + POIOffset.y;
            Position.z = trans.position.z;
            float vibrateIntensity = Mathf.Clamp(shake_intensity / 0.05f, 0, 1);
            
            if (shake_intensity > 0)
            {
                transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
                shake_intensity -= shake_decay;
            }
            else if (bShaking == true)
            {
                transform.rotation = originRotation;
                //ShakeOffset = Vector2.zero;
                bShaking = false;
            }

            //trans.position = Position;
            SmoothTransition();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Cache for position smoothing
    /// </summary>
    Vector3 SmoothPosition = Vector3.zero;

    /// <summary>
    /// Smooths the camera from its current position to position
    /// </summary>
    void SmoothTransition()
    {
        Vector3 diff = Position - trans.position;
        SmoothPosition += diff * SmoothTransitionDelay;
        SmoothPosition.z = trans.position.z;
        trans.position = SmoothPosition;
    }

    /// <summary>
    /// Rate at which the camera moes between two offsets
    /// </summary>
    [Tooltip("Rate at which the camera moes between two offsets")]
    public float SmoothTransitionDelay = 0.3f;

    public float Decay = 0.008f;

    /// <summary>
    /// Begins a screen shake effect for specified strength and duration
    /// </summary>
    /// <param name="_strength">Strength</param>
    /// <param name="_duration">Duration in seconds</param>
    void Shake(float _strength, float _duration)
    {
        if (bShaking == true) return;
        
        bShaking = true;
        //Debug.Log("shaking str = " + _strength + " dur " + _duration);
        //originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = _strength/30.0f;
        shake_decay = shake_intensity *(1/ _duration);
        //Debug.Log("Shake decay = " + shake_decay+ " intensity " + shake_intensity + " FixedT " + Time.fixedDeltaTime + " duration " + _duration);
        shake_decay = shake_decay / (1 /Time.fixedDeltaTime);


        //Debug.Log("Shake decay = " + shake_decay + " FixedT " + Time.fixedTime);
        
    }

    /// <summary>
    /// Screen shake effect for specified strength and duration
    /// </summary>
    /// <param name="_strength">Strength</param>
    /// <param name="_duration">Duration in seconds</param>
    /// <param name="_override">Overides existing effect</param>
    void Shake(float _strength, float _duration, bool _override)
    {
        if (bShaking == true && _override == false) return;

        if (bShaking == false)
            originRotation = transform.rotation;

        shake_intensity = _strength / 30.0f;
        shake_decay = shake_intensity * (1 / _duration);
        //Debug.Log("Shake decay = " + shake_decay+ " intensity " + shake_intensity + " FixedT " + Time.fixedDeltaTime + " duration " + _duration);
        shake_decay = shake_decay / (1 / Time.fixedDeltaTime);

        bShaking = true;
    }

}
