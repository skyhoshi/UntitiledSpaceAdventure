using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Value reference abstract. Used to keep reference to object we are passed.
/// </summary>
/// <typeparam name="T">T class type</typeparam>
class VarRef<T>
{
    private Func<T> _get;
    private Action<T> _set;

    public VarRef(Func<T> @get, Action<T> @set)
    {
        _get = @get;
        _set = @set;
    }

    public T Value
    {
        get { return _get(); }
        set { _set(value); }
    }
}

/// <summary>
/// Watches a selected variable and shows it as a typical progress / hitpoint bar (which you can display in any way)
/// </summary>
public class HealthBar : MonoBehaviour {

    /// <summary>
    /// The image that is the bar itself
    /// </summary>
    [Tooltip("The image that is the bar itself")]
    public Image Bar;

    /// <summary>
    /// Speed in seconds at which the image updates to the new position
    /// </summary>
    [Tooltip("Speed in seconds at which the image updates to the new position")]
    public float AnimateSpeed = 1;

    /// <summary>
    /// Reference to the watched object's value
    /// </summary>
    VarRef<float> mWatchValue = null;

    private float mTargetFill;
    private float mMax;
    private float mChangeCache = float.NaN;

    private bool mAnimating = false;

    /// <summary>
    /// Sets the object variable that the health bar will watch
    /// We may want to watch Player.hitpoints, we would pass in the player component and "hitpoints" as the name to watch
    /// </summary>
    /// <param name="obj">The script in which the value resides E.G. "Player"</param>
    /// <param name="_field">The name of the field which we are watching e.g. "hitpoints" on Player</param>
    /// <param name="_max">Tell the bar what the max value is for the item we are watching</param>
    public void SetWatchValue(object obj, string _field, float _max)
    {
        Type t = obj.GetType();
        bool isProperty = false;
        if (t.GetField(_field) == null)
        {
            if (t.GetProperty(_field) == null)
                throw new System.Exception("Field " + _field + "  does not exist or is private");
            else
                isProperty = true;
        }

        // If it's a not a flot and not a property or if it is a property and not a float throw an exeption
        if (isProperty)
        {
            // Eshew type checks as the type is MonoProperty instead of a base type 
            //if (t.GetProperty(_field).GetType() != typeof(float) )
            //    throw new System.Exception("Field is not a float type: " + t.GetProperty(_field).GetType());
            //else
                mWatchValue = new VarRef<float>(() => (float)t.GetProperty(_field).GetValue(obj, null), val => { t.GetProperty(_field).SetValue(obj, val, null); });
        }
        else
            if (t.GetField(_field).FieldType != typeof(float))
                throw new System.Exception("Field is not a float type: " + t.GetField(_field));
            else
                mWatchValue = new VarRef<float>(() => (float)t.GetField(_field).GetValue(obj), val => { t.GetField(_field).SetValue(obj, val); });

        mChangeCache = mWatchValue.Value;

        SetMaxValue(_max);
    }

    /// <summary>
    /// Set a new maximum value for the bar that we are watching (Player just got an increase in max hitpoints?)
    /// </summary>
    /// <param name="_max">New max</param>
    void SetMaxValue(float _max)
    {
        if (_max <= 0)
        {
            Debug.LogError("Max can not be 0 or negative");
            mMax = 1000;
            return;
        }
        
        mMax = _max;

        ResetBar();
    }

    void Start()
    {
        //SetWatchValue(this, "testValue", 100);
    }

    void OnEnable()
    {
        if (mWatchValue != null)
        {
            ResetBar();
        }
        mAnimating = false;
    }

    /// <summary>
    /// Resets the bars values
    /// </summary>
    private void ResetBar()
    {
        mChangeCache = mWatchValue.Value;
        mTargetFill = mChangeCache / mMax;
        Bar.fillAmount = mTargetFill;
    }

    /// <summary>
    /// Coroutine that handles the animation
    /// </summary>
    IEnumerator AnimateBar()
    {
        mAnimating = true;
        while (Bar.fillAmount != mTargetFill)
        {
            //Debug.Log("Animate Bar");
            Bar.fillAmount += AnimateSpeed * Time.fixedDeltaTime;
            Bar.fillAmount = Mathf.Clamp(Bar.fillAmount, 0, mTargetFill);

            yield return new WaitForFixedUpdate();
        }
        mAnimating = false;
    }

    /// <summary>
    /// Coroutine that handles the animation
    /// </summary>
    IEnumerator AnimateBarDown()
    {
        mAnimating = true;
        while (Bar.fillAmount != mTargetFill)
        {
            Bar.fillAmount -= AnimateSpeed * Time.fixedDeltaTime;
            Bar.fillAmount = Mathf.Clamp(Bar.fillAmount, mTargetFill, mMax);

            yield return new WaitForFixedUpdate();
            
        }
        mAnimating = false;
    }

    void FixedUpdate()
    {
        if (mWatchValue != null && mWatchValue.Value != mChangeCache)
        {
            mChangeCache = mWatchValue.Value;
            mTargetFill = mChangeCache / mMax;
            if (mAnimating == false)
            {
                if (Bar.fillAmount < mTargetFill)
                    StartCoroutine(AnimateBar());
                else
                    StartCoroutine(AnimateBarDown());
            }
        }
    }

}
