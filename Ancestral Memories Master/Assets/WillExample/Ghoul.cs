using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : Enemy
{
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    [SerializeField] LerpParams lerpParams;

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        rend = GetComponent<Renderer>();

        StartCoroutine(BreathingLerpAlpha());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator BreathingLerpAlpha()
    {
        float t = 0f;

        System.Func<float, float> func = Lerp.GetLerpFunction(lerpParams.lerpType);

        while (true)
        {
            while (t <= 1f)
            {
                t += Time.deltaTime / lerpParams.lerpTime;
                rend.material.color = Color.Lerp(startColor, endColor, func(t));
                yield return null;
            }
            // t > 1
            // material is now the endColor

            t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / lerpParams.lerpTime;
                rend.material.color = Color.Lerp(endColor, startColor, func(t));
                yield return null;
            }

            t = 0f;
        }

    }

}
