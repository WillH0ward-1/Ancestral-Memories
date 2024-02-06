using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowersManager : MonoBehaviour
{

    int activeFollowers = 0;
    public List<GameObject> followers;
    public ResourcesManager resourcesManager;

    public void AddFollower(GameObject follower)
    {
        followers.Add(follower);
        // Update ResourcesManager
        if (resourcesManager != null)
        {
            resourcesManager.AddResourceObject("Followers", follower);
        }
    }

    public void RemoveFollower(GameObject follower)
    {
        followers.Remove(follower);
        // Update ResourcesManager
        if (resourcesManager != null)
        {
            resourcesManager.RemoveResourceObject("Followers", follower);
        }
    }

    public void GetActiveFollowers()
    {
        activeFollowers = followers.Count;
    }
}
