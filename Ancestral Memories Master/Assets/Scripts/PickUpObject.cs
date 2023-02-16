using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject pickedUpObject;
    public Transform rightHand;
    private ScaleControl growControl;

    [SerializeField] private PlayerSoundEffects playerAudioSFX;

    private void Awake()
    {
        playerAudioSFX = transform.GetComponent<PlayerSoundEffects>();
    }

    public void PickUpItem() {

        if (pickedUpObject.transform.CompareTag("Mushrooms"))
        {
            playerAudioSFX.UprootPlantEvent();
        }

        growControl = pickedUpObject.GetComponent<ScaleControl>();

        float x = pickedUpObject.transform.localScale.x;
        float y = pickedUpObject.transform.localScale.y;
        float z = pickedUpObject.transform.localScale.z;

        float newX = x / 2;
        float newY = y / 2;
        float newZ = z / 2;

        Vector3 targetScale = new Vector3(newX, newY, newZ);

        StartCoroutine(growControl.LerpScale(pickedUpObject, pickedUpObject.transform.localScale, targetScale, 10, 0));

        pickedUpObject.transform.SetParent(rightHand, true);
        pickedUpObject.transform.localPosition = Vector3.zero;
    }


    public void DestroyPickup()
    {

        Destroy(pickedUpObject);
        
    }

  
}
