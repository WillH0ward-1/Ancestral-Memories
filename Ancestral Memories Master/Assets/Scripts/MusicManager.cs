using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;
using FMOD;
using System.Runtime.InteropServices;
using System.Linq;

public class MusicManager : MonoBehaviour
{

    public enum Modes
    {
        Major,
        Aeolian,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Locrian,
        Klezmer,
        SeAsian,
        Chromatic
    }

    public Dictionary<Modes, string[]> ModeInfo = new()
    {
        { Modes.Major, new string[] { "C", "D", "E", "F", "G", "A", "B" } },
        { Modes.Aeolian, new string[] { "C", "D", "Ds", "F", "G", "Gs", "As" } },
        { Modes.Dorian, new string[] { "C", "D", "Ds", "F", "G", "A", "As" } },
        { Modes.Phrygian, new string[] { "C", "Cs", "Ds", "F", "G", "Gs", "As" } },
        { Modes.Lydian, new string[] { "C", "D", "E", "Fs", "G", "A", "B" } },
        { Modes.Mixolydian, new string[] { "C", "D", "E", "F", "G", "A", "As" } },
        { Modes.Locrian, new string[] { "C", "Cs", "Ds", "F", "Fs", "Gs", "As" } },
        { Modes.Klezmer, new string[] { "C", "D", "Ds", "Fs", "G", "A", "As" } },
        { Modes.SeAsian, new string[] { "C", "C#", "E", "F", "G", "G#", "B" } },
        { Modes.Chromatic, new string[] { "C", "Cs", "D", "Ds", "E", "F", "Fs", "G", "Gs", "A", "As", "B" } }
    };

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
    private EventInstance musicInstance;

    private float minTimeInterval;
    private float maxTimeInterval;

    [SerializeField] private float output = 0;
    [SerializeField] private int faithModulateOutput;
    [SerializeField] private int parameterCount = 0;

    [SerializeField] private string currentlyPlaying;
    [SerializeField] private int currentMode;

    private bool autoModulating = false;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;
    [SerializeField] private Player player;

    FMOD.Studio.EVENT_CALLBACK callbackDelegate;

    Modes randomMode;
    public Modes startingMode = Modes.Chromatic;
    public int voiceCount = 4;

    //StartCoroutine(IntroLoop());

    // musicParameterID = musicParameterDescription.id;
    //globalParamTrigger = transform.GetComponent<StudioGlobalParameterTrigger>();
    // globalParamTrigger.TriggerEvent = EmitterGameEvent.ObjectStart;
    //globalParamTrigger.Parameter = "Mode";

    //studioEventEmitter = transform.GetComponent<StudioEventEmitter>();

    //globalParamTrigger.Value = 5;

    //PlayMusic();

    /*
    musicInstance.getDescription(out EventDescription description);
    description.getParameterDescriptionCount(out int count);
    parameterCount = count;
    newMax = Enum.GetValues(typeof(Modes)).Length;
    */

    private void Start()
    {
        callbackDelegate = new EVENT_CALLBACK(DialogueEventCallback);
        StartCoroutine(RandomWait());
        //globalParamTrigger.Value = (float)GetLabelIndex(musicInstance);

        SetMode(startingMode);
        StartCoroutine(RandomModulateBuffer());
        //StartCoroutine(FaithModulate());
    }

    public IEnumerator RandomWait()
    {
        float buffer = UnityEngine.Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(buffer);

        //RandomModulate();
        StartCoroutine(PlayNote("Strings", voiceCount));

        StartCoroutine(RandomWait());

        yield break;
    }

    public float minRandomModulateWait = 6f;
    public float maxRandomModulateWait = 30f;

    public IEnumerator RandomModulateBuffer()
    {
        float buffer = UnityEngine.Random.Range(minRandomModulateWait, maxRandomModulateWait);
        yield return new WaitForSeconds(buffer);

        RandomModulate();

        yield break;
    }


    public void RandomModulate()
    {
        randomMode = (Modes)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Modes)).Length);

        SetMode(randomMode);

        StartCoroutine(RandomModulateBuffer());
    }

    public void ManualModulate(Modes newMode)
    {
        currentlyPlaying = newMode.ToString();
        currentMode = (int)newMode;

        autoModulating = false;

        SetMode(newMode);

        RuntimeManager.StudioSystem.setParameterByName("Mode", currentMode);

    }

    public IEnumerator WaitForBreak(IEnumerator running)
    {
        if (autoModulating)
        {
            StopCoroutine(running);
        }
        
        yield break;
    }

    public EventReference eventPath;
    public string instrumentFileRootName = "Instrument";
    public string[] notesToUse;

    public float minWait = 10f;
    public float maxWait = 15f;

    public float minDuration = 5f;
    public float maxDuration = 15f;

    public Modes currentModeSetting;

    public void SetMode(Modes mode)
    {
        if (mode != currentModeSetting)
        {
            currentModeSetting = mode;

            notesToUse = ModeInfo[mode];

            for (int i = activeVoices.Count - 1; i >= 0; i--)
            {
                Tuple<EventInstance, string> activeVoice = activeVoices[i];
                EventInstance instrumentInstance = activeVoice.Item1;
                string noteName = activeVoice.Item2;

                if (!notesToUse.Contains(noteName))
                {
                    activeVoices.RemoveAt(i);
                    activeNotes.RemoveAt(i);
                    instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    UnityEngine.Debug.Log("Removed " + noteName + " as this note isn't in the " + mode + " mode!");
                }
            }
        }
    }

    public int maxVoices = 4;
    [SerializeField] private List<Tuple<EventInstance, string>> activeVoices = new();
    [SerializeField] private List<string> activeNotes;

    public static string[] YatesShuffle(string[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, array.Length);
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    public IEnumerator PlayNote(string instrument, int voices)
    {
       // float buffer = UnityEngine.Random.Range(minWait, maxWait);
       // yield return new WaitForSeconds(buffer);

        for (int i = 0; i < Mathf.Min(maxVoices, voices); i++){

            notesToUse = YatesShuffle(notesToUse);

            string note = null;

            foreach (string possibleNote in notesToUse)
            {
                if (!activeNotes.Contains(possibleNote))
                {
                    note = possibleNote;
                    break;
                }
            }

            if (note == null)
            {
                yield break;
            }

            string key = instrumentFileRootName + "/" + instrument + "/" + note;

            UnityEngine.Debug.Log(key);

            EventInstance instrumentInstance = RuntimeManager.CreateInstance(eventPath);

            var activeVoice = Tuple.Create(instrumentInstance, note);

            UnityEngine.Debug.Log(instrumentInstance);

            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            instrumentInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            instrumentInstance.setCallback(callbackDelegate);
            //dialogueInstanceRef = dialogueInstance;

            //dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            instrumentInstance.start();
            instrumentInstance.release();

            activeVoices.Add(activeVoice);
            activeNotes.Add(activeVoice.Item2);

            StartCoroutine(NoteDuration(instrumentInstance, activeVoice));

            yield return null;
            
        }
        
        yield break;
    }

    public IEnumerator NoteDuration(EventInstance instrumentInstance, Tuple<EventInstance, string> activeVoice)
    {
        float duration = UnityEngine.Random.Range(minDuration, maxDuration);

        bool noteEnded = false;

        while (!noteEnded)
        {
            RESULT timeLinePos = instrumentInstance.getTimelinePosition(out int timelinePosition);

            float timelineSeconds = timelinePosition / 1000f;

            if (timeLinePos == RESULT.OK && timelineSeconds >= duration)
            {
                activeVoices.Remove(activeVoice);
                activeNotes.Remove(activeVoice.Item2);
                instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                noteEnded = true;
            }

            yield return null;
        }

        UnityEngine.Debug.Log(instrumentInstance + "ended!");

        yield break;
    }

    public IEnumerator Retrigger(float buffer, int voices)
    {
        yield return new WaitForSeconds(buffer);

        RandomModulate();
        StartCoroutine(PlayNote("Strings", voices));

        yield break;
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

    // Code from FMOD examples https://www.fmod.com/docs/2.02/unity/examples-programmer-sounds.html

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT DialogueEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        // Get the string object
        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        String key = stringHandle.Target as String;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
        }
        return FMOD.RESULT.OK;
    }

}

