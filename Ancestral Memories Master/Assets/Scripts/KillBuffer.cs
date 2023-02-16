using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBuffer : MonoBehaviour
{
    public TreeDeathManager treeDeathManager;
    public float lifeTime = 60;

    
    private void Awake()
    {
        treeDeathManager = transform.GetComponent<TreeDeathManager>();
    }

    public IEnumerator ExpiryBuffer()
    {
        yield return new WaitForSeconds(lifeTime);
        treeDeathManager.Fall();

        yield break;
    }
}
