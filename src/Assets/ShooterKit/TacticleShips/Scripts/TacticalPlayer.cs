using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tactical skill wrapper for skills
/// </summary>
public class TacticalSkill : Skill
{
    public Transform WeaponPrefab;
    public float Duration = 1.0f;
}

/// <summary>
/// Homing attack data contains homing target. 
/// UPDATE: this is now redundant, targets were added to AttackData base however I have left it here to show an example of how to overide the base attack data for your
/// own purposes.
/// </summary>
public class HomingAttackData : AttackData
{
    public Transform Target;
}

/// <summary>
/// Player class for tactical ships example
/// </summary>
public class TacticalPlayer : MonoBehaviour {

    /// <summary>
    /// True when ship is disabled
    /// </summary>
    [Tooltip("True when ship is disabled")]
    public bool bShipDisabled = false;

    #region PlayerEventStuff
    public delegate void PlayerDead(TacticalPlayer _p);
    public event PlayerDead PlayerDeadEvent;

    #endregion

    private CameraFollow mCam;
    private Shooter mShooter;

    /// <summary>
    /// Target for homing bolts
    /// </summary>
    [Tooltip("Target for homing bolts")]
    public Transform Target;

    /// <summary>
    /// Destructable object for this ship
    /// </summary>
    [Tooltip("Destructable object for this ship")]
    public Destructable chassis;

    /// <summary>
    /// List of skills available for the player
    /// </summary>
    [Tooltip("List of skills available for the player")]
    public Skill[] Skills;

    /// <summary>
    /// Primary weapon. Set to Space key
    /// </summary>
    [Tooltip("Primary weapon. Set to Space key")]
    public WeaponConfig WeaponOne;

    /// <summary>
    /// Secondary weapon. Set to E key
    /// </summary>
    [Tooltip("Secondary weapon. Set to E key")]
    public WeaponConfig WeaponTwo;

    /// <summary>
    /// Tertiary weapon set to R
    /// </summary>
    [Tooltip("Tertiary weapon set to R")]
    public WeaponConfig WeaponThree;

    /// <summary>
    /// This weapon is not set to a key!
    /// </summary>
    [Tooltip("This weapon is not set to a key!")]
    public WeaponConfig WeaponFour;

    /// <summary>
    /// Primary weapon fireports
    /// </summary>
    [Tooltip("Primary weapon fireports")]
    public Transform[] WeaponOnePorts;

    /// <summary>
    /// Secondary weapon fireports
    /// </summary>
    [Tooltip("Secondary weapon fireports")]
    public Transform[] WeaponTwoPorts;

    void Start()
    {
        mShooter = GetComponent<Shooter>();
        if (mShooter == null)
            Debug.LogError("Shooter object requires a shooter component");

        mCam = Camera.main.GetComponent<CameraFollow>();
        mCam.target = transform;
    }

    private HomingAttackData mAttackData;

    void OnEnable()
    {

        chassis.DamageTakenEvent += new Destructable.DamageTaken(chassis_DamageTakenEvent);
        for (int i = 0; i < Skills.Length; i++)
        {
            Skills[i].SkillActivatedEvent += new Skill.SkillActivated(TacticalPlayer_SkillActivatedEvent);
        }

        mAttackData = new HomingAttackData();
        mAttackData.Target = Target;
    }

    /// <summary>
    /// Handler for activated skill
    /// </summary>
    /// <param name="_skill"></param>
    void TacticalPlayer_SkillActivatedEvent(Skill _skill)
    {
        if (_skill.SkillName == "Armor Repair")
            RepairArmorTriggered(_skill);
        else
            RegenerateShieldTriggered(_skill);
    }

    /// <summary>
    /// Triggers repair armor effect
    /// </summary>
    /// <param name="_skill"></param>
    void RepairArmorTriggered(Skill _skill)
    {
        StartCoroutine(RepairArmorRoutine(_skill));
    }

    /// <summary>
    /// Repair armor is an effect that lasts for a period of time
    /// </summary>
    /// <param name="_skill">Skill data</param>
    IEnumerator RepairArmorRoutine(Skill _skill)
    {
        TacticalHitConfig t = chassis.Config as TacticalHitConfig;
        if (t == null)
        {
            Debug.Log("Chassis has wrong type");
            yield break;
        }

        float time = 0;
        while (time < _skill.Duration)
        {
            yield return new WaitForFixedUpdate();

            t.Armor += Time.fixedDeltaTime * 25.5f;
            time += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Triggers shield regenerate ability
    /// </summary>
    /// <param name="_skill"></param>
    void RegenerateShieldTriggered(Skill _skill)
    {
        TacticalHitConfig t = chassis.Config as TacticalHitConfig;
        if (t == null)
        {
            Debug.Log("Chassis has wrong type");
            return;
            //yield break;
        }

        t.Shield += 300;
    }

    /// <summary>
    /// Handler for damage taken event
    /// </summary>
    /// <param name="_damageEventArgs"></param>
    void chassis_DamageTakenEvent(System.EventArgs _damageEventArgs)
    {
        mCam.StartScreenShake(0.2f, 0.2f);
    }

    int WeaponOnePort = 0;
    int WeaponTwoPort = 0;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) == true)
        {
            mShooter.SetFirePorts(WeaponOnePorts);
            mShooter.PortCount = WeaponOnePort; // assign the last port we used
            bool Success = mShooter.Fire(WeaponOne, mAttackData);
            WeaponOnePort = mShooter.PortCount; // remember what the current port is   
        }
        if (Input.GetKey(KeyCode.E) == true)
        {
            mShooter.SetFirePorts(WeaponTwoPorts);
            mShooter.PortCount = WeaponTwoPort;
            bool Success = mShooter.Fire(WeaponTwo);
            WeaponTwoPort = mShooter.PortCount;
        }
        if (Input.GetKey(KeyCode.R) == true)
        {
            mShooter.SetFirePorts(WeaponOnePorts);
            mShooter.PortCount = WeaponOnePort;
            bool Success = mShooter.Fire(WeaponThree, mAttackData);
            WeaponOnePort = mShooter.PortCount;

        }
        if (Input.GetKey(KeyCode.R) == true)
        {
            mShooter.SetFirePorts(WeaponOnePorts);
            mShooter.PortCount = WeaponOnePort;
            bool Success = mShooter.Fire(WeaponFour);
            WeaponOnePort = mShooter.PortCount;

        }
    }

    void OnDestroy()
    {
        mCam.StartScreenShake(0.9F, 0.4F);

        Vector3 pos = transform.position;
        pos.z = mCam.transform.position.z;
        mCam.transform.position = pos;
        mCam.target = mCam.transform;

        if (PlayerDeadEvent != null)
            PlayerDeadEvent(this);
    }
}
