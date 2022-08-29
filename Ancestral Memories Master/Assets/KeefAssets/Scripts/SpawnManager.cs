using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    ChunkSpawner chunkSpawner;

    // Start is called before the first frame update
    void Start()
    {
        chunkSpawner = GetComponent<ChunkSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTriggerEntered()
    {
        chunkSpawner.MoveSegment();
    }
}
