using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private Transform target;

    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;

    [SerializeField] private float yOffset = 45f;

    public bool lightningActive = false;

    Light lightningLight;

    public CharacterBehaviours behaviours;
    private DisasterManager naturalDisaster;

    private float lightIntensity = 15f;

    private LightningSoundEffects lightningSFX;

    // Start is called before the first frame update

    private void Awake()
    {
        lightningLight = transform.GetComponentInChildren<Light>();
        lightningLight.transform.gameObject.SetActive(false);
    }

    public void StrikeLightning(Transform target)
    {
        if (!lightningActive)
        {
            //StartCoroutine(behaviours.Electrocution());
            //lightningSFX.PlayLightningStrike();
            StartCoroutine(Strike(target.transform, duration));
        } 
    }

    private IEnumerator Strike(Transform target, float duration)
    {
        lightningActive = true;

        lightningLight.transform.gameObject.SetActive(true);
        lightningLight.intensity = lightIntensity;

        Debug.Log("Lightning!");

        GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity, target.transform);

        lightning.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        lightning.transform.localScale = minScale;

        float time = 0;

        while (time <= 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(minScale, maxScale, time);
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
            time += Time.deltaTime / duration;
            yield return null;
        }

        if (time >= 1f)
        {
            lightningActive = false;
            StartCoroutine(naturalDisaster.DisasterCoolDown());
            Destroy(lightning);
            yield break;
        }
    }
}
