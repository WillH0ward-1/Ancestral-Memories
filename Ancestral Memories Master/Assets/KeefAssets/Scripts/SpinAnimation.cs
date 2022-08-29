using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAnimation : MonoBehaviour
{
    private float tCycle = 0;

    public float multiplier = 0;
 
 void Update()
    {
        float t = Time.time;
        if (t > tCycle) tCycle = t + 3;
        if (tCycle - t <= 1)
        { // Spins during the last second
            transform.Rotate(180, 0, 0 * Time.deltaTime * multiplier);
        }
    }

}
