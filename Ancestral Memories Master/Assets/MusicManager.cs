using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;
using FMOD;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference MusicEventPath;
    private StudioGlobalParameterTrigger globalParamTrigger;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;

    public string currentlyPlaying;

    public enum Modes
    {
        Major,
        NaturalMinor,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Locrian,
        Klezmer,
        SeAsian,
        Chromatic
    }

    public enum GameStates
    {
        Menu,
        Spawning,
        Exploring,
        Praying,
        FluteMode,
        Reflecting,
        Pause,
        Death
    }

    public float psychedeliaParam;

    public enum Locations
    {
        Outside,
        InsideCave,
    }

    public string currentGameState;

    private EventInstance musicInstance;
    private PARAMETER_ID musicParameterID;

    private FMOD.Studio.EventDescription musicEventDescription;

    public bool sequence;

    public float minInterval;
    public float maxInterval;

    [SerializeField] private int currentMode;

    [SerializeField] private bool musicFaithMode = false;
    private bool modulated = false;

    [SerializeField] float faith;

    private StudioEventEmitter studioEventEmitter;

    private void Start()
    {

        //StartCoroutine(IntroLoop());

        // musicParameterID = musicParameterDescription.id;
        //globalParamTrigger = transform.GetComponent<StudioGlobalParameterTrigger>();
       // globalParamTrigger.TriggerEvent = EmitterGameEvent.ObjectStart;
        //globalParamTrigger.Parameter = "Mode";

        //studioEventEmitter = transform.GetComponent<StudioEventEmitter>();

        //globalParamTrigger.Value = 5;

        PlayMusic();
        StartCoroutine(IntroLoop());

        //globalParamTrigger.Value = (float)GetLabelIndex(musicInstance);


        //StartCoroutine(MusicFaithModifier());
    }

    private void Update()
    {
        
    }

    public IEnumerator IntroLoop()
    {
        RuntimeManager.StudioSystem.setParameterByName("State", currentMode);

        minInterval = 7;
        maxInterval = 10;

        ManualModulate(Modes.Major);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.NaturalMinor);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Lydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Phrygian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Mixolydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Locrian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Klezmer);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        StartCoroutine(IntroLoop());

        yield break;
    }

    public IEnumerator Exploring()
    {
        minInterval = 5;
        maxInterval = 10;

        ManualModulate(Modes.Lydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Mixolydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        StartCoroutine(Exploring());

        yield break;
    }

    public IEnumerator Dialogue()
    {
        minInterval = 5;
        maxInterval = 10;

        ManualModulate(Modes.Lydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Mixolydian);

        yield break;
    }

    public void ManualModulate(Modes newMode)
    {
        currentlyPlaying = newMode.ToString();
        currentMode = (int)newMode;

        if (autoModulating)
        {
            StopCoroutine(FaithModulate(0));
            autoModulating = false;
        }

        RuntimeManager.StudioSystem.setParameterByName("Mode", currentMode);

    }

    private bool autoModulating = false;

    public IEnumerator FaithModulate(int newMode)
    {
        autoModulating = true;

        float minKarma = 0;
        float maxKarma = 100;

        while (autoModulating)
        {
            currentMode = newMode;

            //faith = player.faith;

            musicInstance.getDescription(
             out EventDescription description
             );

            description.getParameterDescriptionCount(
            out int count
            );

            float newMin = 0;
            float newMax = count;

            if (newMode != currentMode)
            {
                var t = Mathf.InverseLerp(minKarma, maxKarma, faith);
                int output = (int)Mathf.Lerp(newMin, newMax, t);

                //musicInstance.setParameterByNameWithLabel("Mode", newMode);

            }

            yield return null;
        }

        yield break;
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

