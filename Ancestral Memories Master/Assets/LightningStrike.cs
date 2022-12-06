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

    [SerializeField] private AreaManager areaManager;

    [SerializeField] private GameObject fire;
    [SerializeField] private FireController fireManager;

    private string insideCave = "InsideCave";

    // Start is called before the first frame update

    private void Awake()
    {
        lightningLight = transform.GetComponentInChildren<Light>();
        lightningLight.enabled = false;
    }

    public void StrikeLightning(Transform target)
    {
        if (areaManager.currentRoom != insideCave)
        {
            StartCoroutine(behaviours.Electrocution());
            StartCoroutine(Strike(target));
        } else
        {
            return;
        }

    }

    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;

    public IEnumerator Strike(Transform target)
    {
        maxScale = new Vector3(0.5f, Random.Range(-1f, -20f), 0.5f);

        lightningLight.enabled = true;
        lightningLight.intensity = 0f;

        Debug.Log("Lightning!");

        GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity, target.transform);
        lightningSFX = lightning.GetComponent<LightningSoundEffects>();
        lightningSFX.PlayLightningStrike(target.transform.gameObject);

        lightning.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        lightning.transform.localScale = minScale;

        float halfTime = duration / 2; 

        float lightningDuration = halfTime;

        float time = 0;

        while (time <= 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(minScale, maxScale, time);
            lightningLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, time);
            time += Time.deltaTime / lightningDuration;
            yield return null;
        }

        if (time >= 1f)
        {
            fireManager.StartFire(target.transform, new Vector3(target.position.x, target.position.y, target.position.z));

            yield return Retreat(lightning, lightningDuration);
            
        }

    }

    private IEnumerator Retreat(GameObject lightning, float lightningDuration)
    {
        Debug.Log("Lightning End!");

        lightningLight.transform.gameObject.SetActive(false);
        lightningLight.intensity = 0f;

        float time = 0;

        while (time <= 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(maxScale, minScale, time);
            lightningLight.intensity = Mathf.Lerp(maxLightIntensity, minLightIntensity, time);
            time += Time.deltaTime / lightningDuration;
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
