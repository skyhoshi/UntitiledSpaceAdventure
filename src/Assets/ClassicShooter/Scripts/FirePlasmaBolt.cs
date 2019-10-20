using UnityEngine;
using System.Collections;

/// <summary>
/// Fires the plasma bolt as a charge weapon, inherits from AIFireIfFacing
/// </summary>
public class FirePlasmaBolt : AIFireIfFacing
{
    float time = 8;
    public float Warmup = 2;
    private float warmTime = 0;
    private bool StartFiring = false;
    public Transform WarmupPrefab;
    private Transform warmEffect;

    public float BoltDelay = 1;

    public override void Fire()
    {
        if (time > BoltDelay)
            PlasmaBolt(1); // Useing a fudge angle for now.
        else
            time += Time.fixedDeltaTime;
    }

    /// <summary>
    /// Cancels the effect after an ammount of time. 
    /// Catch all for if AIFireIfFacing aborts the shot
    /// </summary>
    /// <returns></returns>
    IEnumerator PlasmaBoltCancel()
    {
        yield return new WaitForSeconds(Warmup + 0.03f);
        StartFiring = false;
        warmTime = 0;
        time = 0;
    }

    /// <summary>
    /// Called continuously while in a "firing" position. Keeps a timer for warming up the shot
    /// Fires when the warmup time completes
    /// </summary>
    /// <param name="diff">Difference in angle</param>
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
                time = 0;
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
}
