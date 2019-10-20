using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple HitConfig for a single hitpoint value
/// Handles recieving damage from damage dealers
/// </summary>
public  class SimpleHitpoints : HitConfigBase
{
    /// <summary>
    /// Current hitpoints of this unit
    /// </summary>
    public float Hitpoints;

    /// <summary>
    /// Do not create on-the-fly
    /// </summary>
    public SimpleHitpoints()
    {
        System.Exception e = new System.Exception();
        //Debug.Log("simple created " + special + " name: " + name + " at " + e.StackTrace);
        //throw new System.Exception("wrong location");
    }

    /// <summary>
    /// Main function. Recieves damage from damage dealer, sets whether the unit is dead or not
    /// </summary>
    /// <param name="_d"></param>
    /// <returns></returns>
    public override bool RecieveDamage(DamageDealer _d)
    {
        if (gameObject.tag == "Player")
        {
            Debug.Log(string.Format("Taking {0} damage, current hp {1}", _d.Damage.ToString(), Hitpoints.ToString()));
        }

        Hitpoints -= _d.Damage;

        if (Hitpoints <= 0)
            mIsDead = true;

        return true;
    }

}

/// <summary>
/// Base class for Hitconfigs. Defines what functions a hit config should have
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Destructable))]
public abstract class HitConfigBase : MonoBehaviour
{
    void Reset()
    {
        Debug.Log("Reset called by component " + this.GetType());
        GetComponent<Destructable>().SetConfig(this);
    }

    protected bool mIsDead = false;

    /// <summary>
    /// Returns whether this unit is dead or not
    /// </summary>
    public bool IsDead
    {
        get { return mIsDead; }
    }

    /// <summary>
    /// Virtual deal damage function
    /// </summary>
    /// <param name="_d">object dealing damage to this unit</param>
    /// <returns></returns>
    public virtual bool RecieveDamage(DamageDealer _d)
    {
        Debug.LogError("DealDamage must be implimented on the class " + this.GetType());
        return false;
    }

}
