using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWater : MonoBehaviour
{
    [SerializeField] private PlayerSoundEffects soundFX;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Water"))
        {
            soundFX.waterColliding = true;
        }
        else if (!other.transform.CompareTag("Water"))
        {
            soundFX.waterColliding = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
