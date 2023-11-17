using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.RVO;

public class NavMeshCutting : MonoBehaviour
{
    public RVOSquareObstacle obstacle;

    void Awake()
    {
        obstacle = GetComponent<RVOSquareObstacle>();
    }

    public void EnableNavMeshCut()
    {
        if (obstacle != null)
        {
            obstacle.enabled = true;
        }
    }

    public void DisableNavMeshCut()
    {
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }
}
