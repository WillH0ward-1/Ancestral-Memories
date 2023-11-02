using System;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class FireSpreadRange : MonoBehaviour
{
    public enum FlammableTag
    {
        Trees,
        Human,
        Animal
    }



    public float checkInterval = 0.5f;  // How often to check for flammable objects
    public float checkRadius = 10f;     // The radius of the sphere used to check for objects

    private HashSet<GameObject> objectsOnFire = new HashSet<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(CheckForFlammableObjects), checkInterval, checkInterval);
    }

    private void CheckForFlammableObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (var hitCollider in hitColliders)
        {
            GameObject hitObj = hitCollider.gameObject;
            if (IsFlammable(hitObj) && !IsAlreadyOnFire(hitObj))
            {
                if (hitObj.transform.CompareTag("Trees"))
                {
                    FireManager.Instance.StartFireOnSegments(hitObj.transform.GetComponentInChildren<ProceduralTree>().segmentObjects);
                }
                else
                {
                    FireManager.Instance.StartFireOnObject(hitObj);
                }
                objectsOnFire.Add(hitObj); // Mark as on fire
            }
        }
    }

    private bool IsFlammable(GameObject obj)
    {
        return Enum.TryParse(obj.tag, out FlammableTag _);
    }

    private bool IsAlreadyOnFire(GameObject obj)
    {
        return objectsOnFire.Contains(obj);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the check area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
