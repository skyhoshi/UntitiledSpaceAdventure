using UnityEngine;
using System.Collections;

/// <summary>
/// Trigger the shooter to fire continuously
/// </summary>
public class ExampleContinuousFire : MonoBehaviour {

    private Shooter mShooter;
    public Transform BulletPrefab;
    private WeaponConfig WeaponInfo;
    void Start()
    {
        
        mShooter = gameObject.GetComponent<Shooter>();
        WeaponInfo = BulletPrefab.GetComponent<WeaponConfig>();
    }

    float tElapsed = 0;
    float tDelay = 0.5f;
    void FixedUpdate()
    {
        if (WeaponInfo)
            mShooter.Fire(BulletPrefab);
        else if (tElapsed > tDelay)
        {
            mShooter.Fire(BulletPrefab);
            tElapsed = 0;
        }

        tElapsed += Time.fixedDeltaTime;
    }
}
