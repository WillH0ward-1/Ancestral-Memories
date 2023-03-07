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

    [SerializeField] private EventReference MusicEventPath;
    private EventInstance musicInstance;

    [SerializeField] private float output = 0;
    [SerializeField] private int faithModulateOutput;

    [SerializeField] private string currentlyPlaying;
    [SerializeField] private int currentMode;

    private bool autoModulating = false;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;
    [SerializeField] private Player player;

    EVENT_CALLBACK callbackDelegate;

    Modes randomMode;
    public Modes startingMode = Modes.Chromatic;
    public int stringsVoiceCount = 4;

    public TimeCycleManager timeCycleManager;

    public CharacterBehaviours characterBehaviours;

    public enum Instruments
    {
        Strings,
        PlateScrapeSynth
    }

    private void Start()
    {

        callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
        SetMode(startingMode);
        StartCoroutine(NoteBuffer());
        StartCoroutine(TimeJuggle());
        StartCoroutine(RandomModulateBuffer());

        //StartCoroutine(FaithModulate());
    }

    public IEnumerator NoteBuffer()
    {
        float buffer = UnityEngine.Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(buffer);

        switch (areaManager.currentRoom)
        {
            case "Outside" when characterBehaviours.isPsychdelicMode:
                StartCoroutine(PlayNote(GetRandomInstrument().ToString(), stringsVoiceCount, null));
                break;
            case "Outside":
                StartCoroutine(PlayNote("Strings", stringsVoiceCount, null));
                break;
            case "InsideCave":
                StartCoroutine(PlayNote("PlateScrapeSynth", stringsVoiceCount, null));
                break;
            default:
                StartCoroutine(PlayNote("Strings", stringsVoiceCount, null));
                break;
        }

        StartCoroutine(NoteBuffer());

        yield break;
    }

    public Instruments GetRandomInstrument()
    {
        Instruments randomInstrument;
        int randomIndex = UnityEngine.Random.Range(0, Enum.GetValues(typeof(Instruments)).Length);
        randomInstrument = (Instruments)randomIndex;

        return randomInstrument;
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

                    string newNote = GetClosestValidNote(noteName);

                    StartCoroutine(PlayNote("Strings", 1, newNote));
   
                    instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                    UnityEngine.Debug.Log("Removed " + "'" + noteName + "'" + " as this note isn't in the " + mode + " mode! " + newNote + " used instead!");
                }
            }
        }
    }

    public string GetClosestValidNote(string noteName)
    {
        string[] chromaticScale = ModeInfo[Modes.Chromatic];
        int noteIndex = Array.IndexOf(chromaticScale, noteName);

        int newIndex;

        if (UnityEngine.Random.value >= 0.5f)
        {
            newIndex = noteIndex + 1;
        }
        else
        {
            newIndex = noteIndex - 1;
        }

        return chromaticScale[newIndex % chromaticScale.Length];
    }

    public int maxVoices = 4;
    [SerializeField] private List<Tuple<EventInstance, string>> activeVoices = new();
    [SerializeField] private List<string> activeNotes;

    public static string[] YatesShuffle(string[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            System.Random random = new System.Random();

            int randomNumber = random.Next(0, array.Length); 
            int randomIndex = randomNumber;
            string currentIndex = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = currentIndex;
        }
        return array;
    }

    public IEnumerator PlayNote(string instrument, int voices, string manualPitch)
    {
        // float buffer = UnityEngine.Random.Range(minWait, maxWait);
        // yield return new WaitForSeconds(buffer);

        notesToUse = YatesShuffle(notesToUse);

        for (int i = 0; i < Mathf.Min(maxVoices, voices); i++){

            string note = null;

            if (manualPitch != null)
            {
                note = manualPitch;
            }

            else
            {
                manualPitch = null;

                foreach (string validNote in notesToUse)
                {
                    if (!activeNotes.Contains(validNote))
                    {
                        note = validNote;
                        break;
                    }
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

            float timelineSeconds = timelinePosition / 1000f; // convert from ms to seconds

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
        StartCoroutine(PlayNote("Strings", voices, null));

        yield break;
    }


    public IEnumerator TimeJuggle()
    {
        float minTimeJuggleBuffer = UnityEngine.Random.Range(2, 6);
        float maxTimeJuggleBuffer = UnityEngine.Random.Range(minDuration, 6);

        yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeJuggleBuffer, maxTimeJuggleBuffer));

        minDuration = UnityEngine.Random.Range(2, 6);
        maxDuration = UnityEngine.Random.Range(minDuration, 6);
        float duration = Mathf.Clamp(maxDuration, minDuration, 6);
        StartCoroutine(TimeJuggle());

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

