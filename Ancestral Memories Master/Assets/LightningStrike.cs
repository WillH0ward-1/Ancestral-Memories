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

    // Start is called before the first frame update
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
        Debug.Log("Lightning!");

        GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity, target.transform);

        lightning.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        lightning.transform.localScale = minScale;

        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            lightning.transform.localScale = Vector3.Lerp(minScale, maxScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= duration)
        {
            yield return new WaitForSeconds(lightningDuration);
            yield return Retreat(lightning, duration);
        }

    }

    private IEnumerator Retreat(GameObject lightning, float duration)
    {
        Debug.Log("Lightning End!");

        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            lightning.transform.localScale = Vector3.Lerp(maxScale, minScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= duration)
        {
            Destroy(lightning);
            lightningActive = false;
            yield break;
        }
    }
}
