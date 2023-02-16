using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;
using FMOD;

public class MusicManager : MonoBehaviour
{
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

    public enum Locations
    {
        Outside,
        InsideCave,
    }

    public string currentGameState;

    [SerializeField] private EventReference MusicEventPath;
    private StudioEventEmitter studioEventEmitter;
    private EventInstance musicInstance;

    private float minInterval;
    private float maxInterval;

    [SerializeField] private float output = 0;
    [SerializeField] private int faithModulateOutput;
    [SerializeField] private int parameterCount = 0;

    [SerializeField] private string currentlyPlaying;
    [SerializeField] private int currentMode;

    private bool autoModulating = false;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;
    [SerializeField] private Player player;

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

        musicInstance.getDescription(out EventDescription description);
        description.getParameterDescriptionCount(out int count);
        parameterCount = count;
        newMax = Enum.GetValues(typeof(Modes)).Length;

        //globalParamTrigger.Value = (float)GetLabelIndex(musicInstance);


        StartCoroutine(FaithModulate());
    }

    private void Update()
    {
        
    }


    public IEnumerator WaitForBreak(IEnumerator running)
    {
        if (autoModulating)
        {
            StopCoroutine(running);
        }
        
        yield break;
    }

    public IEnumerator MusicIntro()
    {
        StartCoroutine(WaitForBreak(MusicIntro()));

       // RuntimeManager.StudioSystem.setParameterByName("State", currentMode);

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

        StartCoroutine(MusicIntro());

        yield break;
    }

    public IEnumerator MusicExploring()
    {
        StartCoroutine(WaitForBreak(MusicExploring()));

        minInterval = 5;
        maxInterval = 10;

        ManualModulate(Modes.Lydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        ManualModulate(Modes.Mixolydian);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minInterval, maxInterval));

        StartCoroutine(MusicExploring());

        yield break;
    }

    public IEnumerator MusicDialogue()
    {
        StartCoroutine(WaitForBreak(MusicDialogue()));

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

        autoModulating = false;

        RuntimeManager.StudioSystem.setParameterByName("Mode", currentMode);

    }

    public IEnumerator FaithModulate()
    {
        autoModulating = true;

        while (autoModulating)
        {
            //faith = player.faith;

            RuntimeManager.StudioSystem.setParameterByName("Mode", currentMode);
            //musicInstance.setParameterByNameWithLabel("Mode", newMode);

            yield return null;
        }

        yield break;
    }

    float newMin;
    float newMax;

    private void OnEnable()
    {

        player.OnFaithChanged += KarmaModifier;

    }

    private void OnDisable()
    {

        player.OnFaithChanged -= KarmaModifier;
    }

    private void KarmaModifier(float karma, float minKarma, float maxKarma)
    {
        float newMin = 0;


        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        output = Mathf.Lerp(newMin, newMax, t);
        faithModulateOutput = (int)Mathf.Floor(output);
        currentMode = faithModulateOutput;
    }

    void PlayMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(MusicEventPath);

        musicInstance.start();
        musicInstance.release();

    }

    void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

}

