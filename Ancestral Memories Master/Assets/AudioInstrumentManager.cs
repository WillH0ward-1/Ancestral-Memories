using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInstrumentManager : MonoBehaviour
{
    CsoundUnity cSoundObj;

    string channelONOFF = "ONOFF";
    string channelNoteC = "C";
    string channelNoteCs = "Cs";
    string channelNoteD = "D";
    string channelNoteDs = "Ds";
    string channelNoteE = "E";
    string channelNoteF = "F";
    string channelNoteFs = "Fs";
    string channelNoteG = "G";
    string channelNoteGs = "Gs";
    string channelNoteA = "A";
    string channelNoteAs = "As";
    string channelNoteB = "B";
    string channelStrength = "Strength";
    string channelHarmonics = "Harmonics";
    string channelLFODepth = "LFODepth";
    string channelLFOSpeed = "LFOSpeed";
    string channelPitch = "Pitch";
    string channelOctave = "Octave";
    string channelStability = "Stability";
    string channelBreath = "Breath";
    string channelGkFreq = "gkFreq";
    string channelAmpAttack = "AmpAttack";
    string channelAmpSustain = "AmpSustain";
    string channelAmpDecay = "AmpDecay";
    string channelAmpRelease = "AmpRelease";

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

    public enum Octaves
    {
        Oct0 = 0,
        Oct1 = 1,
        Oct2= 2,
        Oct3 = 3,
        Oct4= 4
    }


    public Dictionary<Modes, string[]> ModeInfo = new()
    {
        { Modes.Major, new string[] { "C", "D", "E", "F", "G", "A", "B"} },
        { Modes.Aeolian, new string[] { "C", "D", "Ds", "F", "G", "Gs", "As"} },
        { Modes.Dorian, new string[] { "C", "D", "Ds", "F", "G", "A", "As"} },
        { Modes.Phrygian, new string[] { "C", "Cs", "Ds", "F", "G", "Gs", "As"} },
        { Modes.Lydian, new string[] { "C", "D", "E", "Fs", "G", "A", "B"} },
        { Modes.Mixolydian, new string[] { "C", "D", "E", "F", "G", "A", "As"} },
        { Modes.Locrian, new string[] { "C", "Cs", "Ds", "F", "Fs", "Gs", "As"} },
        { Modes.Klezmer, new string[] { "C", "D", "Ds", "Fs", "G", "A", "As"} },
        { Modes.SeAsian, new string[] { "C", "Cs", "E", "F", "G", "Gs", "B"} },
        { Modes.Chromatic, new string[] { "C", "Cs", "D", "Ds", "E", "F", "Fs", "G", "Gs", "A", "As", "B" } }
    };

    public Modes currentMode = Modes.Major; // Default mode
    public Octaves currentOctave = Octaves.Oct2;

    public string[] notesToUse;

    private void Awake()
    {
        cSoundObj = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        SetOctave(Octaves.Oct3);
        SetMode(Modes.Major);
    }

    public void SetOctave(Octaves octave)
    {
        currentOctave = octave;
        // Cast the enum to int to get its numeric value
        int octaveValue = (int)octave;
        // Now you can use octaveValue to set the parameter
        CabbageAudioManager.Instance.SetParameter(cSoundObj, channelOctave, octaveValue);
    }

    public void SetMode(Modes mode)
    {
        currentMode = mode;
        notesToUse = ModeInfo[mode];
    }

    public void PlayNote(string note)
    {
        SetNote(note);
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, channelONOFF, true));
    }

    public void SetNote(string note)
    {
        if (notesToUse == null || notesToUse.Length == 0) return;

        // Trigger the corresponding note channel
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, note, true));
    }

    [SerializeField]
    private float minNoteDuration = 1f; // Minimum duration for a note

    [SerializeField]
    private float maxNoteDuration = 5f; // Maximum duration for a note


    private Coroutine melodyCoroutine; // To keep track of the melody coroutine

    public void PlayRandomMelody()
    {
        if (melodyCoroutine != null)
        {
            StopCoroutine(melodyCoroutine);
        }
        melodyCoroutine = StartCoroutine(PlayMelodyCoroutine());
    }

    private IEnumerator PlayMelodyCoroutine()
    {
        while (true) // Loop indefinitely
        {
            string[] shuffledNotes = ShuffleArray(notesToUse);
            foreach (string note in shuffledNotes)
            {
                PlayNote(note);

                // Wait for a random duration before playing the next note
                float duration = Random.Range(minNoteDuration, maxNoteDuration);
                yield return new WaitForSeconds(duration);
            }
        }
    }

    public void StopFlute()
    {
        if (melodyCoroutine != null)
        {
            StopCoroutine(melodyCoroutine);
            melodyCoroutine = null;
        }

        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, channelONOFF, false));
    }

    private string[] ShuffleArray(string[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);
            string temp = array[i];
            array[i] = array[rnd];
            array[rnd] = temp;
        }
        return array;
    }


    void Update()
    {
        
    }
}
