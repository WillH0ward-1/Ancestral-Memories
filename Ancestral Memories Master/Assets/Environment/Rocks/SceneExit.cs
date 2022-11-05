using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExit : MonoBehaviour
{


    public string sceneToLoad;

    bool inTrigger;
    float delayTimer;
    Collider objectCollider;

    public void Start()
    {
        delayTimer = 3f;
        inTrigger = false;
    }
    public void Open()
    {
        SendMessageUpwards("LoadRoom");
        objectCollider.enabled = false;
        delayTimer = 4f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;

            if (delayTimer > 0)
            {
                return;
            }

            Open();

        }
        else
        {
            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;

            if (delayTimer > 0)
            {
                return;
            }

            Open();

        } else
        {
            return;
        }
    }
}
