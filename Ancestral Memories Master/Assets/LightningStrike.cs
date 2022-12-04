using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;

    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;

    [SerializeField] private float yOffset = 45f;

    public bool lightningActive = false;

    Light lightningLight;

    public CharacterBehaviours behaviours;
    [SerializeField] private DisasterManager naturalDisaster;

    private LightningSoundEffects lightningSFX;

    private ControlAlpha costumeControl;

    // Start is called before the first frame update

    private void Awake()
    {
        lightningLight = transform.GetComponentInChildren<Light>();
        lightningLight.enabled = false;
    }

    public void StrikeLightning(Transform target)
    {
       
        StartCoroutine(behaviours.Electrocution());
        StartCoroutine(Strike(target));

    }


    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;

    public IEnumerator Strike(Transform target)
    {

        lightningLight.enabled = true;
        lightningLight.intensity = 0f;

        Debug.Log("Lightning!");

        GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity, target.transform);
        lightningSFX = lightning.GetComponent<LightningSoundEffects>();
        lightningSFX.PlayLightningStrike(target.transform.gameObject);

        lightning.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        lightning.transform.localScale = minScale;

        float halfTime = duration / 2; 

        duration = halfTime;

        float time = 0;

        while (time <= 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(minScale, maxScale, time);
            lightningLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, time);
            time += Time.deltaTime / duration;
            yield return null;
        }

        if (time >= 1f)
        {
            yield return Retreat(lightning, duration);
            
        }

    }

    private IEnumerator Retreat(GameObject lightning, float duration)
    {
        Debug.Log("Lightning End!");

        lightningLight.transform.gameObject.SetActive(false);
        lightningLight.intensity = 0f;

        float time = 0;

        while (time <= 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(maxScale, minScale, time);
            lightningLight.intensity = Mathf.Lerp(maxLightIntensity, minLightIntensity, time);
            time += Time.deltaTime / duration;
            yield return null;
        }

        if (time >= 1f)
        {
            lightningLight.intensity = 0f;
            lightningActive = false;
            StartCoroutine(naturalDisaster.DisasterCoolDown());
            Destroy(lightning);
            yield break;
        }
    }
}
