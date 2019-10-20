using UnityEngine;
using System.Collections;


/// <summary>
/// Bespoke script to control the tactical enemy player
/// </summary>
public class TacticalEnemy : MonoBehaviour {

    /// <summary>
    /// List of available weapons
    /// </summary>
    [Tooltip("List of available weapons")]
    public WeaponConfig[] Weapons;

    /// <summary>
    /// Fireports on the side of the ship
    /// </summary>
    [Tooltip("Fireports on the side of the ship")]
    public Transform[] FirePortsSide;

    /// <summary>
    /// Fireports on the front of the ship
    /// </summary>
    [Tooltip("Fireports on the front of the ship")]
    public Transform[] FirePortsFront;

    /// <summary>
    /// Time between launching barrage of bolts
    /// </summary>
    [Tooltip("Time between launching barrage of bolts")]
    public float BoltTime = 5;

    /// <summary>
    /// Time between launching barrage of bolts
    /// </summary>
    [Tooltip("Time between launching barrage of bolts")]
    public float BoltDuration = 5;

    /// <summary>
    /// Port number
    /// </summary>
    private int BoltPortNo;

    private Shooter mShooter;

    public void OnEnable()
    {
        mShooter = GetComponent<Shooter>();
        //StartCoroutine(Bolts());
        mAttackData = new HomingAttackData();
        mAttackData.Target = Target;
    }

    void FixedUpdate()
    {
        Bolts();
    }

    /// <summary>
    /// Handles firing torpedo (not enabled currently)
    /// </summary>
    void Torpedo()
    {
        float nextFire = Random.RandomRange(1, BoltTime);
        bool isFiring = false;
        while (true)
        {
            //yield return new WaitForFixedUpdate();
            if (isFiring)
            {
                nextFire += Time.fixedDeltaTime;
                mShooter.SetFirePorts(FirePortsSide);
                mShooter.Fire(Weapons[0]);
                if (nextFire > BoltDuration)
                {
                    nextFire = Random.RandomRange(3, BoltTime + 3);
                    isFiring = false;
                }
            }
            else
            {
                nextFire -= Time.fixedDeltaTime;
                if (nextFire <= 0)
                {
                    nextFire = 0;
                    isFiring = true;
                }
            }
        }

    }
    float nextFire = 3;
    bool isFiring = false;

    private HomingAttackData mAttackData;

    /// <summary>
    /// Fire's lots of little homing bolts using it's selected target
    /// </summary>
    void Bolts()
    {
        if (isFiring)
        {
            nextFire += Time.fixedDeltaTime;
            mShooter.SetFirePorts(FirePortsSide);
            mShooter.PortCount = BoltPortNo;
            bool Success = mShooter.Fire(Weapons[0], mAttackData);
            BoltPortNo = mShooter.PortCount;

            if (nextFire > BoltDuration)
            {
                nextFire = Random.RandomRange(3, BoltTime + 3);
                isFiring = false;
            }
        }
        else
        {
            nextFire -= Time.fixedDeltaTime;
            if (nextFire <= 0)
            {
                nextFire = 0;
                isFiring = true;
            }
        }
    }

    /// <summary>
    /// Currently selected target
    /// </summary>
    public Transform Target;
    

}
