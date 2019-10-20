using UnityEngine;
using System.Collections;
//using XInputDotNetPure;

/// <summary>
/// Player object for a classic shooter game
/// </summary>
public class Player : MonoBehaviour {

    #region PlayerEventStuff
    public delegate void PlayerDead(Player _p);
    public event PlayerDead PlayerDeadEvent;

    #endregion

    /// <summary>
    /// Used to get the current state of the player
    /// </summary>
    public bool bShipDisabled = false;

    /// <summary>
    /// Primary fire
    /// </summary>
    public Transform        WeaponPrefab;

    /// <summary>
    /// Alternate fire
    /// </summary>
    public Transform        WeaponPrefab2;

    /// <summary>
    /// The component that keeps track of whether the player is alive
    /// </summary>
    public Destructable     chassis;

    /// <summary>
    /// Display for players current health
    /// </summary>
    public HealthBar        healthBar;

    /// <summary>
    /// Main camera (used to trigger camera shake effects)
    /// </summary>
    public Transform        mainCanvas;

    private CameraFollow    mCam;
    private Shooter         mShooter;

    /// <summary>
    /// How much weapon energy the player can have at maximum
    /// </summary>
    public float MaxEnergy;

    /// <summary>
    /// Current energy level
    /// </summary>
    public float Energy;

    /// <summary>
    /// Primary fire energy cost. Edit on prefab
    /// </summary>
    public float EnergyCostLaser            = 1.75f;

    /// <summary>
    /// Alternate fire energy cost. Edit on prefab
    /// </summary>
    public float EnergyCostAutcannon        = 25.6f;

    /// <summary>
    /// Energy regenerated per second. Edit on prefab
    /// </summary>
    public float EnergyRegen                = 1500.9465f;

    void Start()
    {
        mShooter = GetComponent<Shooter>();
        if (mShooter == null)
            Debug.LogError("Shooter object requires a shooter component");

        mCam = Camera.main.GetComponent<CameraFollow>();
        mCam.target = transform;

        if (healthBar)
        {   // Set up healthbar watch 
            SimpleHitpoints hp = chassis.Config as SimpleHitpoints;
            healthBar.SetWatchValue(hp, "Hitpoints", hp.Hitpoints);
            healthBar.transform.SetParent(mainCanvas, true);
        }
        else    
            Debug.LogError("Healthbar is missing");
    }

    void OnValidate()
    {
        Debug.Log("Validating player");
    }

    void OnEnable()
    {
        chassis.DamageTakenEvent += new Destructable.DamageTaken(chassis_DamageTakenEvent);
    }

    void chassis_DamageTakenEvent(System.EventArgs _damageEventArgs)
    {
        mCam.StartScreenShake(0.2f, 0.2f);
    }

    void OnDestroy()
    {
        mCam.StartScreenShake(0.9F, 0.4F);

        // Remposition the camera.
        Vector3 pos = transform.position;
        pos.z = mCam.transform.position.z;
        mCam.transform.position = pos;
        mCam.target = mCam.transform;
        
        // Remove healthbar from the canvas
        if (healthBar != null)
            Destroy(healthBar.gameObject);

        // Trigger events
        if (PlayerDeadEvent != null)
            PlayerDeadEvent(this);
    }

    /// <summary>
    /// Triggers the camera shake efffect
    /// </summary>
    /// <param name="_intensity">How "big" the shakes are</param>
    /// <param name="_duration">Shake duration</param>
    /// <param name="_override">Override a previous shake</param>
    public void ShakeScreen(float _intensity, float _duration, bool _override)
    {
        mCam.StartScreenShake(_intensity, _duration, _override);
    }

    float cooldown = 0.05f;
    float cooldown2 = 0.02f;
    float timePassed = 0;

    void FixedUpdate()
    {
        
        timePassed += Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.Space) == true || Input.GetAxis("Fire1") > 0)
        {
            if (timePassed > cooldown && HasEnergy(EnergyCostLaser))
            {
                mShooter.Fire(WeaponPrefab);
                //GamePad.SetVibration(PlayerIndex.One, 0, 0.1f);
                timePassed = 0;
                UseEnergy(EnergyCostLaser);
            }
            
        }
        else if (Input.GetKey(KeyCode.E) == true || Input.GetAxis("Fire2") > 0)
        {
            if (timePassed > cooldown2 && HasEnergy(EnergyCostAutcannon))
            {
                if (mShooter.Fire(WeaponPrefab2))
                {
                    timePassed = 0;
                    UseEnergy(EnergyCostAutcannon);
                }
                //GamePad.SetVibration(PlayerIndex.One, 0, 0.3f);
                
            }

        }

        ssTime += Time.fixedDeltaTime;

        //if (state.Buttons.X == ButtonState.Pressed && ssTime > ssCooldown)
        //{
        //    Application.CaptureScreenshot("ClusterFrak" + "." + numShots + ".jpg");
        //    numShots++;
        //    ssTime = 0;
        //}

        EnergyRegenerate();
    }

    float numShots = 0;
    float ssCooldown = 0.5f;
    float ssTime = 0;

    /// <summary>
    /// Test for if the player has enough energy to trigger the action of spceified value
    /// </summary>
    /// <param name="_value">threshold value</param>
    /// <returns></returns>
    public bool HasEnergy(float _value)
    {
        return (_value <= Energy);
    }

    public float LastEnergyUsedTime;

    /// <summary>
    /// Remove specified ammount of energy from the pool
    /// </summary>
    /// <param name="_value">Ammount to remove</param>
    public void UseEnergy(float _value)
    {
        Energy -= _value;
        LastEnergyUsedTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Delay before regenerating energy
    /// </summary>
    public float EnergyRegenTime = 0.6857f;


    /// <summary>
    /// Tick for regenerating energy
    /// </summary>
    public void EnergyRegenerate()
    {
        if (Time.realtimeSinceStartup - LastEnergyUsedTime > EnergyRegenTime)
        {
            Energy += EnergyRegen * Time.fixedDeltaTime;
            Energy = Mathf.Clamp(Energy, 0, MaxEnergy);
        }
    }

    //void OnGUI()
    //{
    //    string text = "";
    //    text += Energy.ToString();
    //    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
    //}
}
