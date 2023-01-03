using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFlute : MonoBehaviour
{
    float distance;
    float distanceThreshold;
    GameObject attenuationObject;
    public Camera cam;
    public Player player;
    public CharacterBehaviours behaviours;
    public LayerMask targetLayer;

    public IEnumerator FluteControl()
    {
        if (!Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonUp(0))
            {
                    yield break;
            }

            void CastRayToGround()
            {

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, targetLayer))
                {
                    Vector3 attenuation = attenuationObject.transform.position;

                    distance = Vector3.Distance(attenuation, rayHit.point);

                    Debug.Log(distance);

                    if (distance >= distanceThreshold)
                    {
                        return;
                    }
                    else
                    {
                        StartCoroutine(PlayNote(rayHit.point, distance, attenuation));
                    }
                }
            }
        }
    }

    public IEnumerator PlayNote(Vector3 point, float distance, Vector3 attenuation)
    {
        while (Input.GetMouseButton(1)) {
            yield return null;
        }

        yield break;
    }
}