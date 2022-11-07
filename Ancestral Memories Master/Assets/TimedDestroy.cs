using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimedDisableCollider(2f));
    }

    private IEnumerator TimedDisableCollider(float time)
    {
        yield return new WaitForSeconds(time);
        var collider = gameObject.GetComponent<Collider>();
        collider.enabled = false;
        // Destroy(gameObject);
        yield break;
    }
}
