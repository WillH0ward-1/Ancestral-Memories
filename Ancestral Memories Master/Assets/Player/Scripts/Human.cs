using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    [SerializeField] private string characterName;
    [SerializeField] private Health health;
    [SerializeField] private Faith faith;
    [SerializeField] private Hunger hunger;
    [SerializeField] private Evolution evolution;
    [SerializeField] private Range range;
    [SerializeField] private bool hasDied = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
