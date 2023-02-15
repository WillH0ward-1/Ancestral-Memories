using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowersManager : MonoBehaviour
{

    int activeFollowers = 0;
    public List<GameObject> followers;

    public void AddFollower(GameObject follower)
    {
        followers.Add(follower);
    }

    public void RemoveFollower(GameObject follower)
    {
        followers.Remove(follower);
    }

    public void GetActiveFollowers()
    {
        activeFollowers = followers.Count;
    }
}
