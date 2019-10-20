using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utils
{
	partial class Utilities
	{
        
        // Changes a portion of _value by the degree of _input as defined by lower and uppwer bounds
        // e.g. rotation could be scaled by how fast a vehicle is going, but only by 28%
        //  LerpPart(minSpeed, maxSpeed, 0.28, TurnRate, currentSpeed)
        public static float LerpPart(float _lowerBound, float _upperBound, float _part, float _value, float _input)
        {   
            float rv = (1 - ( (_input - _lowerBound) / (_upperBound - _lowerBound))) * (_part * _value) + ((1 - _part) * _value);
            return rv;
        }
        public static float LerpPartInv(float _lowerBound, float _upperBound, float _part, float _value, float _input)
        {
            float rv = (( (_input - _lowerBound) / (_upperBound -_lowerBound))) * (_part * _value) + ((1 - _part) * _value);
            return rv;
        }

        public static int RandomSign()
        {
            return UnityEngine.Random.Range(0, 1) * 2 - 1;
        }
        public static float RandomBinamial()
        {
            return UnityEngine.Random.Range(-1.0f, 1.0f) - UnityEngine.Random.Range(0, 1);
        }
        /* Returns ammount to turn for an object trying to face a target over Time.deltaT */
        public static float GetAngleTurnToFace(float _angleDegTarget, float _angleDegCurrent, float _angleDegThreshold, float _rotationDegPerSecond)
        {
            //print = false;
            //if (time <= 0)
            //{
            //    print = true;
            //    time = printTime;
            //}
            //time -= Time.deltaTime;

            //_angleDegCurrent += 90;
            float rotation = Time.deltaTime * _rotationDegPerSecond;

            //Print("Angle Target = " + _angleDegTarget + " Current = " + _angleDegCurrent);
            _angleDegTarget += (-90 + 180);
            _angleDegTarget += 360;
            _angleDegTarget = _angleDegTarget % 360;
            if (_angleDegTarget > 360) { _angleDegTarget -= 360; }
            float diff = Mathf.Abs(_angleDegCurrent - _angleDegTarget);

            float rv = 0;
            if (_angleDegCurrent == _angleDegTarget)
            {
                rv = 0;
            }
            else if (_angleDegCurrent > _angleDegTarget && diff > _angleDegThreshold)
            {
                if (diff < rotation)
                {
                    rotation = diff;
                }

                if (diff > 180)
                {
                    rotation = -rotation;
                }
                rv = -rotation;
            }
            else if (_angleDegCurrent < _angleDegTarget && diff > _angleDegThreshold)
            {
                if (diff < rotation)
                {
                    rotation = diff;
                }

                if (diff > 180)
                {
                    rotation = -rotation;
                }
                rv = rotation;
            }
            //Print("2 Angle Target = " + _angleDegTarget + " Current = " + _angleDegCurrent + " rotation = " + rotation);
            return rv;
        }

	}

    public static class ExtensionMethods
    {
        public static float CrossProduct(this Vector2 _a, Vector2 _b)
        {
            return (_a.x * _b.y) - (_a.y * _b.x);
        }

        /* Projects a by b, to point p returning vector pab */
        public static Vector2 Projection(this Vector2 _a, Vector2 _b)
        {
            Vector2 v = _a.normalized;
            float mag = Vector2.Dot(v, _b) / v.magnitude;
            return mag * v;
        }

        /* Returns the distance to the specified vector */
        public static float DistanceTo(this Vector2 _a, Vector2 _b)
        {
            return (_b - _a).magnitude;
        }

        // Reset a transform to it's normal scale (this is usefull for creating prefabs in NGUI)
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}
