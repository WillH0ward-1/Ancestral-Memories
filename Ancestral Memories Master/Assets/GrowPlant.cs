using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrowPlant : MonoBehaviour
{
    GameObject dirtMound;
    GameObject sapling;

    Vector3 scaleStart = new(0, 0, 0);
    Vector3 scaleDestination = new(1, 2, 1.4f);
    GrowControl grow;
    float duration;

    void PrepareMound()
    {
        duration = 3f;
        grow.Grow(dirtMound, scaleStart, scaleDestination, duration);
    }
}
