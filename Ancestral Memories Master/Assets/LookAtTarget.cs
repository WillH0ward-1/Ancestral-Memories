using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    float y;

    private void Start()
    {
        y = gameObject.transform.rotation.eulerAngles.y;
        y += 180;
    }
    void Update()
    { 
        gameObject.transform.LookAt(target.position);
     
    }
}
