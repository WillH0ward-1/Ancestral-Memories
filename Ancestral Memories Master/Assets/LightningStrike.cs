using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private Transform target;

    [SerializeField] private float lightningSpeed = 1f;
    [SerializeField] private float lightningDuration;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;

    [SerializeField] private float yOffset = 45f;

    private bool lightningActive = false;

    Light lightningLight;

    // Start is called before the first frame update

    private void Awake()
    {
        lightningLight = transform.GetComponentInChildren<Light>();
        lightningLight.transform.gameObject.SetActive(false);
    }
    void StrikeLightning()
    {
        if (!lightningActive)
        {
            lightningActive = true;
            StartCoroutine(Strike(target.transform, lightningSpeed));
        } 
    }

    private IEnumerator Strike(Transform target, float duration)
    {
        lightningLight.transform.gameObject.SetActive(true);
        lightningLight.intensity = 100f;

        Debug.Log("Lightning!");

        GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity, target.transform);

        lightning.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        lightning.transform.localScale = minScale;

        float timeElapsed = 0;

        while (timeElapsed < 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(minScale, maxScale, timeElapsed);
            timeElapsed += Time.deltaTime / duration;
            yield return null;
        }

        if (timeElapsed >= 1f)
        {
            yield return new WaitForSeconds(lightningDuration);
            yield return Retreat(lightning, duration);
        }

    }

    private IEnumerator Retreat(GameObject lightning, float duration)
    {
        Debug.Log("Lightning End!");

        lightningLight.transform.gameObject.SetActive(false);
        lightningLight.intensity = 0f;

        float timeElapsed = 0;

        while (timeElapsed < 1f)
        {
            lightning.transform.localScale = Vector3.Lerp(maxScale, minScale, timeElapsed);
            timeElapsed += Time.deltaTime / duration;
            yield return null;
        }

        if (timeElapsed >= 1f)
        {
            Destroy(lightning);
            lightningActive = false;
            yield break;
        }
    }
}
