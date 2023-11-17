using System;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class FireSpreadRange : MonoBehaviour
{
    public enum FlammableTag { Trees, Human, Animal }

    public float checkInterval = 0.5f;
    public float checkRadius = 10f;
    public LayerMask flammableLayer;  // Set this in the inspector

    private HashSet<GameObject> objectsOnFire = new HashSet<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(CheckForFlammableObjects), checkInterval, checkInterval);
    }

    private void CheckForFlammableObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, flammableLayer);

        foreach (var hitCollider in hitColliders)
        {
            GameObject hitObj = hitCollider.gameObject;

            if (IsFlammable(hitObj))
            {

                if (hitObj.CompareTag("Trees"))
                {
                    if (!hitObj.GetComponentInChildren<PTGrowing>().ValidateTree()) continue;
                }

                if (!IsAlreadyOnFire(hitObj))
                {
                    StartCoroutine(FireManager.Instance.StartFireOnObject(hitObj));
                    objectsOnFire.Add(hitObj);
                }
            }
        }
    }

    private bool IsFlammable(GameObject obj)
    {
        return Enum.TryParse(obj.tag, out FlammableTag _);
    }

    public void ObjectExtinguished(GameObject obj)
    {
        objectsOnFire.Remove(obj);
    }

    private bool IsAlreadyOnFire(GameObject obj)
    {
        return objectsOnFire.Contains(obj);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = objectsOnFire.Count > 0 ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
