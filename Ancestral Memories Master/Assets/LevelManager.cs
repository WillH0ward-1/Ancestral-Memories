using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    PortalScript sourcePortal
    private void Start()
    {
        staticRef = this;
        loadingLevel = false
    }
    public void LoadRoom(PortalScript portal)
    {
        int levelIndex = Random.Range(1, Application.levelCount - 1);
        Application.LoadLevelAdditive(levelIndex);
        bool loadingLevel = true;
        bool sourcePortal = portal;
    }
}
