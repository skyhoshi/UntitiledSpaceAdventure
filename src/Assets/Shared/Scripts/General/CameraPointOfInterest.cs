using UnityEngine;
using System.Collections;

public class CameraPointOfInterest : MonoBehaviour {
    
    /// <summary>
    /// Main camera
    /// </summary>
    public CameraFollow Cam;

    /// <summary>
    /// Weight for how much the camera should pay attention to this object
    /// </summary>
    [Tooltip("Weight for how much the camera should pay attention to this object")]
    public int Weight;

    private Transform trans;

    void Awake()
    {
        trans = transform;
    }

    /// <summary>
    /// Set the camera this POI belongs to
    /// </summary>
    /// <param name="_cam">Camera</param>
    public void SetFollowCamera(CameraFollow _cam)
    {
        Cam = _cam;
    }

    void Start()
    {
        trans = transform;
        if (Cam == null) GetDefaultCamera();
        if (Cam == null) return;
        Cam.AddPOI(trans);
    }

    /// <summary>
    /// Get the default camera in the scene
    /// </summary>
    void GetDefaultCamera()
    {
        Cam = CameraFollow.GetDefaultCamera();
    }

    void OnEnable()
    {
        if (Cam == null) GetDefaultCamera();
        if (Cam) Cam.AddPOI(trans);
    }

    void OnDisable()
    {
        if (Cam == null) GetDefaultCamera();
        Cam.RemovePOI(trans);
    }
}
