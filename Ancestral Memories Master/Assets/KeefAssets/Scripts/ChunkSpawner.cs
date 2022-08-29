using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChunkSpawner : MonoBehaviour
{

    public List<GameObject> segments;

    public float offset = 120f;

    // Start is called before the first frame update
    void Start()
    {
        if (segments != null && segments.Count > 0)
        {
            segments = segments.OrderBy(r => r.transform.position.z).ToList();
        }

    }

    public void MoveSegment()

    {
        GameObject movedChunk = segments[0];

        segments.Remove(movedChunk);

        float newZ = segments[segments.Count - 1].transform.position.z + offset;
        movedChunk.transform.position = new Vector3(0, 0, newZ);

        segments.Add(movedChunk);
    }

    
    // Update is called once per frame
    void Update()
    {
     
        
    }
}
