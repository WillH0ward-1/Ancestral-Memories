using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LerpType
{
    Linear, SmoothStep, SmootherStep, Sinusoidal, EaseIn, Exponential, EaseInOutCubic, EaseInOutCircular
}

public enum BlendType
{
    Simultaneous, OneAfterOther
}

[System.Serializable]
public struct LerpParams
{
    public float lerpTime;
    public LerpType lerpType;
    public BlendType blendType;
}

public static class Lerp
{
    public static float Linear(float t)
    {
        return t;
    }
    public static float SmoothStep(float t)
    {
        return Mathf.Pow(t, 2) * (3f - 2f * t);
    }

    public static float SmootherStep(float t)
    {
        return Mathf.Pow(t, 3) * (t * (6f * t - 15f) + 10f);
    }

    public static float SmootherStep(float t, int exponent)
    {
        return Mathf.Pow(t, exponent) * (t * (6f * t - 15f) + 10f);
    }

    public static float Sinusoidal(float t)
    {
        return Mathf.Sin(t * Mathf.PI * 0.5f);
    }

    public static float EaseIn(float t)
    {
        return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    }

    public static float Exponential(float t)
    {
        return t * t;
    }

    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4 * t * t * t :
            1 - Mathf.Pow(-2 * t + 2, 3) * 0.5f;

    }

    public static float EaseInOutCircular(float t)
    {
        return t < 0.5f ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) * 0.5f :
            (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) * 0.5f;
    }

    public static Func<float, float> GetLerpFunction(LerpType type)
    {
        switch (type)
        {
            case LerpType.Linear:
                return Linear;
            case LerpType.SmoothStep:
                return SmoothStep;
            case LerpType.SmootherStep:
                return SmootherStep;
            case LerpType.Sinusoidal:
                return Sinusoidal;
            case LerpType.EaseIn:
                return EaseIn;
            case LerpType.Exponential:
                return Exponential;
            case LerpType.EaseInOutCubic:
                return EaseInOutCubic;
            case LerpType.EaseInOutCircular:
                return EaseInOutCircular;
            default:
                return Sinusoidal;
        }
    }

    
}