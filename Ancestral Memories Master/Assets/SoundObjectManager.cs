using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class SoundObjectManager : MonoBehaviour
{
    [SerializeField] private int initialPoolSize = 100;
    [SerializeField] private int maxPoolSize = 200;
    [SerializeField] private List<GameObject> soundObjectPool;

    void Awake()
    {
        // Initialize the sound object pool
        soundObjectPool = new List<GameObject>(initialPoolSize);
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject soundObject = new GameObject("Sound");
            soundObject.hideFlags = HideFlags.HideInHierarchy;
            soundObject.SetActive(false);
            soundObjectPool.Add(soundObject);
        }
    }

    public GameObject RequestSoundObject()
    {
        GameObject availableSoundObject = null;

        foreach (var soundObject in soundObjectPool)
        {
            if (!soundObject.activeInHierarchy)
            {
                availableSoundObject = soundObject;
                availableSoundObject.SetActive(true);
                break;
            }
        }

        if (availableSoundObject == null && soundObjectPool.Count < maxPoolSize)
        {
            GameObject soundObject = new GameObject("SoundEmitter");
            soundObject.hideFlags = HideFlags.HideInHierarchy;
            soundObjectPool.Add(soundObject);
            availableSoundObject = soundObject;
        }

        return availableSoundObject;
    }

    public void ReleaseSoundObject(GameObject soundObject)
    {
        soundObject.SetActive(false);
    }
}
