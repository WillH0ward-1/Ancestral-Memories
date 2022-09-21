using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Human
{
    private int minStat = 0;
    private int maxStat = 100;

    [SerializeField] public bool killedByGod = false;

    void Start()
    {
        Human player = new Human();
        Debug.Log(player);

    }

    void Update()
    {

    }
}