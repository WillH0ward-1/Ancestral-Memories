using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{

    public void SetSpawnPosition(List<GameObject> spawnPointsList)
    {
        int randomIndex = Random.Range(0, spawnPointsList.Count); // Get a random index within the range of the list
        GameObject randomSpawnPoint = spawnPointsList[randomIndex]; // Get the spawn point at that index
        transform.position = randomSpawnPoint.transform.position; // Set the position of the transform to the position of the randomly selected spawn point
    }

}
