using System.Collections;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject pickedUpObject;
    public Transform rightHand;
    private ScaleControl growControl;
    public MushroomGrowth mushroomControl;

    private string handTag = "PickUpHand";
    [SerializeField] private AudioSFXManager playerAudioSFX;

    private void Awake()
    {
        playerAudioSFX = transform.GetComponent<AudioSFXManager>();
        rightHand = FindChildByTag(transform, handTag).transform;
    }

    public void PickUpItem()
    {
        if (pickedUpObject.transform.CompareTag("Mushrooms"))
        {
            playerAudioSFX.UprootPlantEvent();
            mushroomControl = pickedUpObject.transform.GetComponent<MushroomGrowth>();
            mushroomControl.StopAllGrowthProcesses();
        }

        growControl = pickedUpObject.GetComponent<ScaleControl>();
        if (growControl != null)
        {
            float x = mushroomControl.growScale.x;
            float y = mushroomControl.growScale.y;
            float z = mushroomControl.growScale.z;

            pickedUpObject.transform.Rotate(0f, 0f, 90f);

            Vector3 targetScale = new Vector3(x / 2, y / 2, z / 2);

            StartCoroutine(growControl.LerpScale(pickedUpObject, pickedUpObject.transform.localScale, targetScale, 10, 0));
        }

        pickedUpObject.transform.SetParent(rightHand, true);
        pickedUpObject.transform.localPosition = Vector3.zero;
    }

    public void DestroyPickup()
    {
        Destroy(pickedUpObject);
    }

    private GameObject FindChildByTag(Transform parent, string tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }

            GameObject result = FindChildByTag(child, tag);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
