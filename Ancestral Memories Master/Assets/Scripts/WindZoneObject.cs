using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneObject : MonoBehaviour
{
    private WindObjectPool pool;

    private void Awake()
    {
        pool = transform.root.GetComponent<WindObjectPool>();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (pool != null)
        {
            pool.RemoveActiveWindZone(transform);
        }
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        if (pool != null)
        {
            pool.AddActiveWindZone(transform);
        }
    }

    public void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnWindZoneObject(gameObject);
        }
    }
}