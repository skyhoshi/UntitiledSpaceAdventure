using UnityEngine;
using System.Collections;

/// <summary>
/// Ship object for Tactical ships demo
/// </summary>
public class TacticalShip : MonoBehaviour {

    public bool bShipDisabled = false;
    #region Events
    public delegate void ShipDead(TacticalShip TacticalShip);
    public event ShipDead ShipDeadEvent;

    #endregion

    private CameraFollow mCam;

    /// <summary>
    /// Destructable object
    /// </summary>
    public Destructable chassis;

    [Tooltip("Set this to a healthbar in the UI")]
    public HealthBar ShieldBar;
    [Tooltip("Set this to a healthbar in the UI")]
    public HealthBar ArmorBar;
    [Tooltip("Set this to a healthbar in the UI")]
    public HealthBar HullBar;

    /// <summary>
    /// Main camera
    /// </summary>
    public Transform mainCanvas;

    private Shooter mShooter;

    void Start()
    {
        TacticalHitConfig thc = chassis.Config as TacticalHitConfig;

        ShieldBar.SetWatchValue(thc, "Shield", thc.MaxShield);
        ArmorBar.SetWatchValue(thc, "Armor", thc.MaxArmor);
        HullBar.SetWatchValue(thc, "Hull", thc.MaxHull);



        mShooter = GetComponent<Shooter>();
        if (mShooter == null)
            Debug.LogError("Shooter object requires a shooter component");

        mCam = Camera.main.GetComponent<CameraFollow>();
        mCam.target = transform;

        
    }

    void OnEnable()
    {

        chassis.DamageTakenEvent += new Destructable.DamageTaken(chassis_DamageTakenEvent);
    }

    /// <summary>
    /// Damage taken event handler. Unused currently
    /// </summary>
    /// <param name="_damageEventArgs"></param>
    void chassis_DamageTakenEvent(System.EventArgs _damageEventArgs)
    {
        
    }

    void OnDestroy()
    {
        if (ShieldBar != null)
            Destroy(ShieldBar.gameObject);

        if (ArmorBar != null)
            Destroy(ArmorBar.gameObject);

        if (HullBar != null)
            Destroy(HullBar.gameObject);

        if (ShipDeadEvent != null)
            ShipDeadEvent(this);
    }
}
