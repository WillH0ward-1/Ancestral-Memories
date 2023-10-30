using System.Collections;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class TreeBranchSpawner : MonoBehaviour
{
    private ProceduralTree proceduralTree;
    private List<TreeBranch> branches;

    void Start()
    {
        // Get a reference to the ProceduralTree script.
        proceduralTree = GetComponentInChildren<ProceduralTree>();


    }


}
