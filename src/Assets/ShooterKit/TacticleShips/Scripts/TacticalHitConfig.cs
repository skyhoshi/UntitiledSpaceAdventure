using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A more complex example of a hit config that has Shields, Armor and Hull. The unit is considered dead when there is no hull remaining
/// Damage hits shields first then armor and once those are exhausted, then hull. Some weapons may penetrate shields or have bonuses to specific layers
/// </summary>
public class TacticalHitConfig : HitConfigBase
{
    public float MaxShield;
    public float MaxArmor;
    public float MaxHull;

    /// <summary>
    /// Constructor
    /// </summary>
    public TacticalHitConfig()
    {
        //Debug.Log("tactucak created " + special + " name: " + name);
    }

    /// <summary>
    /// Hitpoint valueslist
    /// </summary>
    protected Dictionary<string, float> Values;

    /// <summary>
    /// Initializes value list
    /// </summary>
    private void InitValues()
    {
        //Debug.Log("Initing values");
        Values = new Dictionary<string, float>();
        Values.Add("Shield", MaxShield);
        Values.Add("Armor", MaxArmor);
        Values.Add("Hull", MaxHull);
        //Debug.Log(string.Format("{0} shield, {1} armor, {2} hull", MaxShield, MaxArmor, MaxHull));
    }

    /// <summary>
    /// Get / Set shield value
    /// </summary>
    public float Shield
    {
        get
        {
            if (Values == null)
                InitValues();
            return Values["Shield"];
        }
        set { SetValue("Shield", value); }

    }

    /// <summary>
    /// Get / Set Armor value
    /// </summary>
    public float Armor
    {
        get
        {
            if (Values == null)
                InitValues();
            return Values["Armor"];
        }
        set { SetValue("Armor", value); }

    }

    /// <summary>
    /// Get / Set Hull value
    /// </summary>
    public float Hull
    {
        get
        {
            if (Values == null)
                InitValues();
            return Values["Hull"];
        }
        set { SetValue("Hull", value); }

    }

    /// <summary>
    /// Set value of type
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_value"></param>
    public void SetValue(string _type, float _value)
    {
        if (Values == null)
            InitValues();

        if (Values.ContainsKey(_type) == false)
        {
            Debug.LogError(string.Format("Value type {0} not found in {1}", _type, "ArmoredShipConfig"));
            return;
        }

        float max = 0;
        switch (_type)
        {
            case "Armor": max = MaxArmor; break;
            case "Shield": max = MaxShield; break;
            case "Hull": max = MaxHull; break;
        }

        Values[_type] = Mathf.Min(_value, max);
    }

    /// <summary>
    /// Reset this hitconfig object
    /// </summary>
    public void ResetConfig()
    {
        InitValues();
    }

    /// <summary>
    /// Get one of the values
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public float GetValue(string _type)
    {
        if (Values == null)
            InitValues();

        if (Values.ContainsKey(_type) == false)
        {
            Debug.LogError(string.Format("Value type {0} not found in {1}", _type, "ArmoredShipConfig"));
            return 0;
        }
        //foreach (string key in Values.Keys)
        //{
        //    Debug.Log(string.Format("{0} is {1}", key, Values[key]));
        //}
        return Values[_type];
    }

    /// <summary>
    /// Main function, Recieve damage applies damage from a damage dealer (must be of type ArmoredShipDamageDealer)
    /// </summary>
    /// <param name="_d"></param>
    /// <returns></returns>
    public override bool RecieveDamage(DamageDealer _d)
    {
        ArmoredShipDamageDealer d = _d as ArmoredShipDamageDealer;
        if (d == null)
        {   // This converts a regular DamageDealer into an ArmoredShipDamageDealer
            // In this case you may want to throw an error instead. 
            d = new ArmoredShipDamageDealer();
            d.Damage = _d.Damage;
        }

        if (d.Damage == 0)
            return false;

        if (Values == null)
            InitValues();

        float remainder = Damage("Shield", d, 0);
        remainder = Damage("Armor", d, remainder); // Apply the damage to the next layer
        remainder = Damage("Hull", d, remainder);

        if (Values["Hull"] <= 0)
            mIsDead = true; // Check if the unit is dead

        return true;
    }

    /// <summary>
    /// Applies damage according to proportions and retains the remainnig damage values
    /// Some weapon types deal bonus damage to armor, and will, for instance do double damage vs shields, but not the other layers.
    /// Remaining damage is returned from this function as a float.
    /// </summary>
    /// <param name="_type">Layer damage is being dealt to</param>
    /// <param name="_d">Damage dealer</param>
    /// <param name="_remainder">Remaining damage from previous layer</param>
    /// <returns>Remaining damage</returns>
    protected float Damage(string _type, ArmoredShipDamageDealer _d, float _remainder)
    {
        float proportion = 0;
        float multiplier = 0;
        float max = 0;
        if (_type == "Shield")
        {
            proportion = _d.DamageDistributionShields / (_d.DamageDistributionArmor + _d.DamageDistributionHull + _d.DamageDistributionShields);
            multiplier = _d.DamageMultiplierShields;
            max = MaxShield;
        }
        else if (_type == "Armor")
        {
            proportion = _d.DamageDistributionArmor / (_d.DamageDistributionArmor + _d.DamageDistributionHull + _d.DamageDistributionShields);
            multiplier = _d.DamageMultiplierArmor;
            max = MaxArmor;
        }
        else if (_type == "Hull")
        {
            proportion = _d.DamageDistributionHull / (_d.DamageDistributionArmor + _d.DamageDistributionHull + _d.DamageDistributionShields);
            multiplier = _d.DamageMultiplierHull;
            max = MaxHull;
        }

        float damage = ((proportion * _d.Damage) + _remainder) * multiplier;
        //Debug.Log(string.Format("{0}, {1}, {2}. {3} ",proportion, _d.Damage, _remainder, multiplier));
        float remainingDamage = 0;
        if (damage > Values[_type])
        {
            remainingDamage = (damage - Values[_type]) / multiplier;
            //Debug.Log(string.Format("{0}, {1}, {2}. {3} ", _type, Values[_type], damage, multiplier));
            damage = Values[_type];
        }
        Values[_type] -= damage;
        Values[_type] = Mathf.Clamp(Values[_type], 0, max);

        return remainingDamage;
    }
}
