using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private static StatsManager instance;

    private AICharacterStats[] allStats;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        // Retrieve all AICharacterStats in the scene
        allStats = FindObjectsOfType<AICharacterStats>();
    }

    public static StatsManager Instance
    {
        get { return instance; }
    }

    public AICharacterStats[] GetAllStats()
    {
        return allStats;
    }

    // Other methods as needed
}
