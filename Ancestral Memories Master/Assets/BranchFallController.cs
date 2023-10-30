using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProceduralModeling;

public class BranchFallController : MonoBehaviour
{
    private ProceduralTree treeGrowingScript;

    void Start()
    {
        treeGrowingScript = GetComponent<ProceduralTree>();
        if (treeGrowingScript == null)
        {
            Debug.LogError("PTGrowing script not found on the GameObject");
            return;
        }

        // Start the branch shedding process
        //ShedBranches();
    }

    public void ShedBranches()
    {
        StartCoroutine(ShedBranchRoutine());
    }

    private IEnumerator ShedBranchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);  // Example: shed a branch every 10 seconds
            ShedRandomBranch();
        }
    }

    private void ShedRandomBranch()
    {
        /*
        // Get all end branches (or any other criteria)
        var endBranches = treeGrowingScript.GetAllEndBranches();
        if (endBranches.Count > 0)
        {
            int index = Random.Range(0, endBranches.Count);
            TreeBranch branchToShed = endBranches[index];

            // Implement your logic for what happens when a branch is shed
            // For example: Play an animation, deactivate, etc.
            // Make sure you have access to do this from PTGrowing or directly if TreeBranch is accessible
            //branchToShed.Shed();  // Ensure TreeBranch has a method to handle shedding
        }
        */
    }
}
