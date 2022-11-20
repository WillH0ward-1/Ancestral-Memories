using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolControl : MonoBehaviour
{
    public bool wieldObject = false;
    public bool wielded = false;

    public void Wield(GameObject wieldedObject, GameObject sheathedObject)
    {
        wielded = true;
        wieldedObject.SetActive(true);
        sheathedObject.SetActive(false);
    }


    public void Sheathe(GameObject wieldedObject, GameObject objectSheathed)
    {

        wielded = false;
        wieldedObject.SetActive(false);
        objectSheathed.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
