using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference MusicEventPath;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;
    
    [NonSerialized] public int ModeMajor = 0;
    [NonSerialized] public int ModeNaturalMinor = 1;
    [NonSerialized] public int ModeDorian = 2;
    [NonSerialized] public int ModePhrygian = 3;
    [NonSerialized] public int ModeLydian = 4;
    [NonSerialized] public int ModeMixolydian = 5;
    [NonSerialized] public int ModeLocrian = 6;
    [NonSerialized] public int ModeKlezmer = 7;
    [NonSerialized] public int ModeSeAsian = 8;

    private EventInstance musicInstance;

    private int currentMode;

    private void Start()
    {
        PlayMusic();
        Modulate(ModeMajor);
    }

    public void Modulate(int newMode)
    {
        if (newMode != currentMode)
        {
            currentMode = newMode;

            musicInstance.setParameterByName("Mode", currentMode);
        }

        return;

    }

    void PlayMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(MusicEventPath);

        musicInstance.start();
        musicInstance.release();
    }

    void StopMusic()
    {
        musicInstance.stop(FMODUnity.STOP_MODE.AllowFadeout);
        musicInstance.release();
    }


}
