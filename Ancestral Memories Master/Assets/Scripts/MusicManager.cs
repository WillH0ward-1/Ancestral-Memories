using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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
        { Modes.Major, new string[] { "C", "D", "E", "F", "G", "A", "B", "C2" } },
        { Modes.Aeolian, new string[] { "C", "D", "Ds", "F", "G", "Gs", "As", "C2" } },
        { Modes.Dorian, new string[] { "C", "D", "Ds", "F", "G", "A", "As", "C2" } },
        { Modes.Phrygian, new string[] { "C", "Cs", "Ds", "F", "G", "Gs", "As", "C2" } },
        { Modes.Lydian, new string[] { "C", "D", "E", "Fs", "G", "A", "B", "C2" } },
        { Modes.Mixolydian, new string[] { "C", "D", "E", "F", "G", "A", "As", "C2" } },
        { Modes.Locrian, new string[] { "C", "Cs", "Ds", "F", "Fs", "Gs", "As", "C2" } },
        { Modes.Klezmer, new string[] { "C", "D", "Ds", "Fs", "G", "A", "As", "C2" } },
        { Modes.SeAsian, new string[] { "C", "Cs", "E", "F", "G", "Gs", "B", "C2" } },
        { Modes.Chromatic, new string[] { "C", "Cs", "D", "Ds", "E", "F", "Fs", "G", "Gs", "A", "As", "B", "C2" } }
    };

    [SerializeField] private string currentlyPlaying;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;
    [SerializeField] private Player player;

    public Modes startingMode = Modes.Chromatic;

    public float minRandomModulateWait = 6f;
    public float maxRandomModulateWait = 30f;

    [SerializeField] private float output = 0;
    [SerializeField] private int faithModulateOutput;

    [SerializeField] private EventReference strings_EventPath;
    [SerializeField] private EventReference pianoTail_EventPath;
    [SerializeField] private EventReference plateScrapeSynth_EventPath;
    [SerializeField] private EventReference jawHarp_EventPath;
    [SerializeField] private EventReference hangDrum_EventPath;
    [SerializeField] private EventReference marimba_EventPath;
    [SerializeField] private EventReference playerFlute_EventPath;
    [SerializeField] private EventReference sine_EventPath;
    [SerializeField] private EventReference darkAmbientSwell_EventPath;

    public enum Instruments // The Instruments type containing the names of instruments
                            // This is held in the InstrumentInfo dictionary
    {
        Strings,
        PianoTail,
        PlateScrapeSynth,
        HangDrum,
        JawHarp,
        Marimba,
        PlayerFlute,
        Sine,
        DarkAmbientSwell
    }

    public Dictionary<Instruments, EventReference> InstrumentInfo = new() // Holds the name and FMOD reference
    {
        { Instruments.Strings, new EventReference() },
        { Instruments.PianoTail, new EventReference() },
        { Instruments.PlateScrapeSynth, new EventReference() },
        { Instruments.HangDrum, new EventReference() },
        { Instruments.JawHarp, new EventReference() },
        { Instruments.Marimba, new EventReference() },
        { Instruments.PlayerFlute, new EventReference() },
        { Instruments.Sine, new EventReference() },
        { Instruments.DarkAmbientSwell, new EventReference() }
    };

    public int stringsVoiceCount = 4; // Initialise voice counts here. 
    /*
     * Unused voice count variables, just demonstration:
     * 
    public int PlateScrapeSynthVoiceCount = 1;
    public int HangDrumVoiceCount = 1; 
    public int MarimbaVoiceCount = 1; 
    public int PlayerFluteVoiceCount = 1; 
    public int SineVoiceCount = 1;
    */

    private void SetEventReferences() // Sets all of the event paths in the dictionary so it may be accessed
                                      // as one object, along with it's instrument name
    {
        InstrumentInfo[Instruments.Strings] = strings_EventPath;
        InstrumentInfo[Instruments.PianoTail] = pianoTail_EventPath;
        InstrumentInfo[Instruments.PlateScrapeSynth] = plateScrapeSynth_EventPath;
        InstrumentInfo[Instruments.HangDrum] = hangDrum_EventPath;
        InstrumentInfo[Instruments.JawHarp] = jawHarp_EventPath;
        InstrumentInfo[Instruments.Marimba] = marimba_EventPath;
        InstrumentInfo[Instruments.PlayerFlute] = playerFlute_EventPath;
        InstrumentInfo[Instruments.Sine] = sine_EventPath;
        InstrumentInfo[Instruments.DarkAmbientSwell] = darkAmbientSwell_EventPath;
    }

    public float minBuffer = 10f;
    public float maxBuffer = 15f;

    public float minLoopDuration = 5f;
    public float maxLoopDuration = 15f;

    public Modes currentModeSetting;

    private void Awake()
    {
        SetEventReferences(); // If we want to serialize the FMOD Event References and store them in
                              // a dictionary with their note names, then we need to initialise the dictionary
                              // values.
    }

    EVENT_CALLBACK callbackDelegate;

    private void Start()
    {
        callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);

        SetMode(startingMode);
        StartCoroutine(NoteBuffer());
        //StartCoroutine(TimeJuggle());
        StartCoroutine(RandomModulateBuffer());

    }

    public CharacterBehaviours characterBehaviours;

    public IEnumerator NoteBuffer() // This is where we wait before starting a new note
    {
        float buffer = UnityEngine.Random.Range(minBuffer, maxBuffer);
        yield return new WaitForSeconds(buffer);

        switch (areaManager.currentRoom) // Convenient, readable method of mapping instrumentation/articulation
                                         // when certain conditions are met. This also provides a relatively smooth-step
                                         // between events and progressions. More control over this in future would be nice.
        {
            case "Outside" when characterBehaviours.isPsychdelicMode:
                StartCoroutine(PlayNote(Instruments.Sine.ToString(), stringsVoiceCount, null, true));
                break;
            case "Outside":
                StartCoroutine(PlayNote(Instruments.Strings.ToString(), stringsVoiceCount, null, true));
                break;
            case "InsideCave":
                StartCoroutine(PlayNote(Instruments.PlateScrapeSynth.ToString(), stringsVoiceCount, null, true));
                StartCoroutine(PlayNote(Instruments.DarkAmbientSwell.ToString(), stringsVoiceCount, null, true));
                break;
            default:
                StartCoroutine(PlayNote(Instruments.Strings.ToString(), stringsVoiceCount, null, true));
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

    public IEnumerator RandomModulateBuffer() // Wait a random amount of time between a min and max before modulating 
    {
        float buffer = UnityEngine.Random.Range(minRandomModulateWait, maxRandomModulateWait);
        yield return new WaitForSeconds(buffer);

        RandomModulate();

        yield break;
    }

    Modes randomMode;

    public void RandomModulate()
    {
        randomMode = (Modes)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Modes)).Length);

        SetMode(randomMode);

        StartCoroutine(RandomModulateBuffer()); // Retrigger the random modulation buffer
    }

    // WIP get closest note in the current mode
    // Currently chooses a new note that is either one semitone higher
    // or one semitone lower than the input note index (noteName), based on a
    // 50/50 random chance. 

    public string GetClosestValidNote(string noteName, string[] mode)
    {
        string[] chromaticScale = ModeInfo[Modes.Chromatic];
        int noteIndex = Array.IndexOf(chromaticScale, noteName); // Get the index of this note in the chromatic scale

        int newNoteIndex;
        string newNote;

        do
        {
            if (UnityEngine.Random.value >= 0.5f)
            {
                newNoteIndex = (noteIndex + 1) % chromaticScale.Length;
            }
            else
            {
                newNoteIndex = (noteIndex - 1 + chromaticScale.Length) % chromaticScale.Length; // prevent negative index
            }

            newNote = chromaticScale[newNoteIndex];
            noteIndex = newNoteIndex;

        } while (!mode.Contains(newNote)); // repeat until we find a valid note

        return newNote;
    }

    public EventReference GetInstrumentEvent(string instrumentName)
    {
        if (!Enum.TryParse(instrumentName, out Instruments instrument)) // try to retrieve the corrosponding Instrument 
        {
            Debug.Log("Invalid instrument name!");
        }

        if (!InstrumentInfo.TryGetValue(instrument, out EventReference eventRef))
        {
            Debug.Log("No event reference for " + instrument + " found in " + InstrumentInfo + "!");
        }

        return eventRef;
    }

    public int maxVoices = 4; // Max number of voices active at one time (Polyphony)

    [SerializeField] private Dictionary<EventInstance, string> activeVoices = new Dictionary<EventInstance, string>(); // Holds The instance and its note name
    [SerializeField] private List<string> activeNotes;

    public string instrumentFileRootName = "Instrument";
    public string[] notesToUse;

    public void SetMode(Modes mode) 
    {
        if (mode != currentModeSetting) 
        {
            currentModeSetting = mode; 

            notesToUse = ModeInfo[mode];

            for (int i = activeVoices.Count - 1; i >= 0; i--) 
            {
                KeyValuePair<EventInstance, string> activeVoice = activeVoices.ElementAt(i);
                EventInstance instrumentInstance = activeVoice.Key;

                string noteName = activeVoice.Value;

                if (!notesToUse.Contains(noteName))
                {
                    activeVoices.Remove(instrumentInstance);
                    activeNotes.Remove(noteName);

                    string newNote = GetClosestValidNote(noteName, notesToUse);
                    StartCoroutine(PlayNote(InstrumentInfo[Instruments.Strings].ToString(), 1, newNote, true));

                    instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                    UnityEngine.Debug.Log("Removed " + "'" + noteName + "'" + " as this note isn't in the " + mode + " mode! " + newNote + " used instead!");
                }
            }
        }
    }

    public static string[] Shuffle(string[] array) // Random Shuffle - https://alessandrofama.com/tutorials/fmod/unity/shuffle-playlist
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, array.Length);
            int randomIndex = randomNumber;
            string currentIndex = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = currentIndex;
        }
        return array;
    }

    public IEnumerator PlayNote(string instrument, int voices, string manualPitch, bool looping)
    {
        notesToUse = Shuffle(notesToUse);

        EventReference eventPath = GetInstrumentEvent(instrument);

        for (int i = 0; i < Mathf.Min(maxVoices, voices); i++) // Clamp the number of notes to generate to maxVoices.
                                                              // repeat the iteration 'voices' number of times if voices
                                                              // is smaller than max voices. 
        {
            string note = null; 

            if (manualPitch != null) // If we're setting the pitch manually
                                     // (By not passing in null to manualPitch of PlayNote())
            {
                note = manualPitch;
            }
            else
            {
                manualPitch = null; // Not using manual pitch, continue to generative process

                foreach (string validNote in notesToUse) // For each validNote in the bank of freshly shuffled notes to use
                {
                    if (!activeNotes.Contains(validNote)) // If activeNotes doesn't already contain the validNote,
                                                          // let note become validNote.
                    {
                        note = validNote;
                        break;
                    }
                }
            }

            if (note == null) // So long as it exists
            {
                yield break;
            }


            string key = instrumentFileRootName + "/" + instrument + "/" + note; // Example Directory:
                                                                                 // AncestralMemoriesSFX/NoteBanks/Instrument/Strings/Cs
                                                                                 // This is the path to a string sound in C# pitch (C sharp)
                                                                                 // Check the 'Instrument' folder for more options
                                                                        
            EventInstance instrumentInstance = RuntimeManager.CreateInstance(eventPath); // Adapted from 'Steve The Cube' Project
            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned); 
            instrumentInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            instrumentInstance.setCallback(callbackDelegate);

            //Debug.Log(key);
            //Debug.Log(instrumentInstance);

            activeVoices.Add(instrumentInstance, note); // Create the 'Voice' object (containing the FMOD instance 
                                                        // and the corrosponding note name) to 'active voices'.

            activeNotes.Add(note); // Add note name to its own separate list too.

            instrumentInstance.start();
            instrumentInstance.release();

            if (looping) 
            {
                StartCoroutine(NoteDuration(instrumentInstance, note)); // Optional control for looping instruments to sustain for a
                                                                        // Random period of time determined by 'minLoopDuration' and 'maxLoopDuration'.
            }

            yield return null;

        }

        yield break;
    }

    public IEnumerator NoteDuration(EventInstance instrumentInstance, string note)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minLoopDuration, maxLoopDuration));

        if (instrumentInstance.isValid())
        {
            activeVoices.Remove(instrumentInstance); // Remove instance from the active voices dictionary 
            activeNotes.Remove(note); // Remove note from active note list

            instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        yield break;
    }

    private List<string> notesList;

    public void PlayOneShot(string instrumentName, GameObject emitter, bool isProgrammerEvent) // Use 'Find References', it's called
    {                                                                 // by other scripts
        EventReference eventPath = GetInstrumentEvent(instrumentName);
        Rigidbody rigidbody = emitter.GetComponentInChildren<Rigidbody>();

        int randomIndex = UnityEngine.Random.Range(0, notesToUse.Length - 1);
        string note = notesToUse[randomIndex];

        notesList = notesToUse.ToList();

        EventInstance instrumentInstance = RuntimeManager.CreateInstance(eventPath);

        Debug.Log(instrumentInstance);

        if (isProgrammerEvent)
        {

            string key = instrumentFileRootName + "/" + instrumentName + "/" + note;
            //Debug.Log(key);
            RuntimeManager.AttachInstanceToGameObject(instrumentInstance, emitter.transform, rigidbody);
            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            instrumentInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            instrumentInstance.setCallback(callbackDelegate);

        }
        else
        {
            RuntimeManager.AttachInstanceToGameObject(instrumentInstance, emitter.transform, rigidbody);
        }

        instrumentInstance.start();
        instrumentInstance.release();
    }

    /*
     * 
public IEnumerator Retrigger(float buffer, int voices)
{
    yield return new WaitForSeconds(buffer);

    RandomModulate();
    StartCoroutine(PlayNote(Instruments.Strings.ToString(), voices, null, true));

    yield break;
}



public IEnumerator TimeJuggle()
{
    float minTimeJuggleBuffer = UnityEngine.Random.Range(minDuration, maxDuration);
    float maxTimeJuggleBuffer = UnityEngine.Random.Range(minDuration, maxDuration);

    yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeJuggleBuffer, maxTimeJuggleBuffer));

    minDuration = UnityEngine.Random.Range(minDuration, maxDuration);
    maxDuration = UnityEngine.Random.Range(minDuration, maxDuration);

    StartCoroutine(TimeJuggle());

    yield break;
}

*/
    /*
float newMin;
float newMax;


 * 
 * Unused/Old code that could be reincorperated in future. Uses the Faith Paramater to map the modes 
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
*/


}

