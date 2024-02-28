using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Qkmaxware.Phonetics;
using UnityEngine;
using static Qkmaxware.Phonetics.IPA;

//using FMODUnity;
// using FMOD.Studio;

public class FormantSynthesizer : MonoBehaviour
{
    CsoundUnity cSoundObj;

    private const string instrument = "LoopSound";

    private const string volumeMaster = "kMasterVolume";

    private const string droneToggle = "toggleDrone";
    private const string speakOneShot = "toggleSpeak";

    private const string autoVowelToggle = "autoVowel";
    private const string autoVowelSpeed = "autoSpeed";
    private const string autoVowelRadius = "autoRadius";

    private const string autoVowelRandom = "autoRandom";
    private const string autoVowelRandomRadius= "autoRandomRadius";

    private const string splatterToggle = "splatter";
    private const string hardSplatToggle = "hardSplat";
    private const string splatSpeedSlider = "splatSpeed";

    private const string formantOne = "kFreq1";
    private const string formantTwo = "kFreq2";
    private const string formantThree = "kFreq3";
    private const string formantFour = "kFreq4";
    private const string formantFive = "kFreq5";

    private const string pitch = "tune";
    private const string pitchFine = "fineTune";

    private const string lfoFrequency = "lfoFrequency";
    private const string lfoDepth = "lfoDepth";

    private const string noiseAmount = "noiseAmountSlider";
    private const string toneAmount = "toneVolumeSlider";

    private const string ampAttack = "ampAttack";
    private const string ampDecay = "ampDecay";
    private const string ampSustain = "ampSustain";
    private const string ampRelease = "ampRelease";


    private Dictionary<string, PhonemeInfo> phoneticData = new Dictionary<string, PhonemeInfo>();

    private VocabularyManager VocabularyManager;
    private IPA ipaInstance; // Assuming you have an IPA class, instantiate or get its reference

    private float minAmpRelease = 0.1f;

    // [SerializeField] private EventReference soundEventRef;
    // EventInstance soundEvent;

    //private string eventPath = "event:/NPC/FormantSynthesizer"; // Replace with the actual path of your event

    private void Awake()
    {
        cSoundObj = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        ipaInstance = new IPA(); // Create a new instance of the IPA class or get its reference if it's a singleton
        LoadPhoneticData();
        CabbageAudioManager.Instance.SetParameter(cSoundObj, ampRelease, minAmpRelease);

    }

    private void LoadPhoneticData()
    {
        string ipaIndexPath = Path.Combine(Application.persistentDataPath, "IPAindex.json");

        if (!File.Exists(ipaIndexPath))
        {
            Debug.LogError("IPAindex.json file not found.");
            return;
        }

        string jsonContent = File.ReadAllText(ipaIndexPath);
        Dictionary<string, IPASymbol> deserializedIpaSymbols = JsonConvert.DeserializeObject<Dictionary<string, IPASymbol>>(jsonContent);

        foreach (var entry in deserializedIpaSymbols)
        {
            string character = entry.Key;
            IPASymbol ipaSymbol = entry.Value;

            PhonemeInfo phonemeInfo = new PhonemeInfo
            {
                phoneme = ipaSymbol.Symbol,
                frequencies = new List<int>
            {
                ipaSymbol.F1,
                ipaSymbol.F2,
                ipaSymbol.F3,
                ipaSymbol.F4,
                ipaSymbol.F5
            }
                // Additional properties can be added if necessary.
            };

            phoneticData[character] = phonemeInfo;
        }
    }

    public enum DroneType
    {
        DroneSteady,
        DroneHardSplat,
        DroneSmoothSplat
    }

    public bool isSinging = false;

    public IEnumerator SingDrone(DroneType droneType)
    {
        float autoVowelRadiusAmount = (float)cSoundObj.GetChannel(autoVowelRadius);
        float autoVowelSpeedAmount = (float)cSoundObj.GetChannel(autoVowelSpeed);
        float splatSpeedAmount = (float)cSoundObj.GetChannel(splatSpeedSlider);

        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, droneToggle, true));

        switch (droneType) // Corrected part here, use the parameter name 'droneType'
        {
            case DroneType.DroneSteady:
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, splatterToggle, false));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, hardSplatToggle, false));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, autoVowelToggle, true));

                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, autoVowelRadius, autoVowelRadiusAmount, 0.5f, formantLerpDuration));
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, autoVowelSpeed, autoVowelSpeedAmount, 1f, formantLerpDuration));

                break;
            case DroneType.DroneHardSplat:

                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, splatterToggle, true));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, hardSplatToggle, true));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, autoVowelToggle, false));
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, splatSpeedSlider, splatSpeedAmount, 0.1f, formantLerpDuration));

                break;
            case DroneType.DroneSmoothSplat:

                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, splatterToggle, true));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, hardSplatToggle, false));
                StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, autoVowelToggle, false));
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, splatSpeedSlider, splatSpeedAmount, 0.1f, formantLerpDuration));
                break;
            default:
                break;
        }

        isSinging = true;

        yield return null;
    }

    public IEnumerator StopSingDrone()
    {
        float autoVowelRadiusAmount = (float)cSoundObj.GetChannel(autoVowelRadius);
        float autoVowelSpeedAmount = (float)cSoundObj.GetChannel(autoVowelSpeed);

        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, autoVowelRadius, autoVowelRadiusAmount, 0, formantLerpDuration));
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, autoVowelSpeed, autoVowelSpeedAmount, 0, formantLerpDuration));

        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, droneToggle, false));
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, autoVowelToggle, false));
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, hardSplatToggle, false));

        isSinging = false;

        yield return null;
    }

    public IEnumerator Speak(string word, float speakingSpeed)
    {
        if (isSinging)
        {
            StartCoroutine(StopSingDrone());
        }

        string ipaRepresentation = ipaInstance.EnglishToIPA(word);
        Debug.Log($"Word: {word}, IPA: {ipaRepresentation}");

        List<int[]> allFormants = ipaInstance.GetFormants(ipaRepresentation);
        List<string> phonemes = ExtractPhonemes(ipaRepresentation); // Method to extract individual phonemes

        foreach (var phoneme in phonemes)
        {
            bool isFricative = Formants["Fricatives"].ContainsKey(phoneme);
            int[] formants = Formants.SelectMany(cat => cat.Value)
                                     .Where(p => p.Key == phoneme)
                                     .Select(p => p.Value)
                                     .FirstOrDefault(); // This finds the formants for the phoneme

            if (formants != null)
            {
                // Adjust parameters based on whether the phoneme is a fricative
                SendFreq(formants, speakingSpeed, isFricative);
                yield return new WaitForSeconds(speakingSpeed); // Wait based on speaking speed
            }
        }
    }

    private List<string> ExtractPhonemes(string ipaRepresentation)
    {
        // This method should parse the ipaRepresentation string to extract individual phonemes
        // For simplicity, here we just convert each character into a phoneme
        // For more complex IPA notations, a more sophisticated parsing might be necessary
        return ipaRepresentation.Select(c => c.ToString()).ToList();
    }


    private List<string> ExtractPhonemesFromIPA(string ipaRepresentation)
    {
        List<string> phonemes = new List<string>();
        // Extract phonemes from the IPA representation and add them to the 'phonemes' list
        // This might involve parsing the string based on known phoneme patterns or using a predefined mapping
        return phonemes;
    }


    public float formantLerpDuration = 0.1f;

    private void SendFreq(int[] frequencies, float duration, bool isFricative)
    {
        if (frequencies.Length >= 5)
        {
            float breathAmoujnt = (float)cSoundObj.GetChannel(noiseAmount);
            float vocalToneAmount = (float)cSoundObj.GetChannel(toneAmount);

            if (isFricative)
            {
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, toneAmount, vocalToneAmount, 0f, formantLerpDuration));
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, noiseAmount, breathAmoujnt, 1, formantLerpDuration));
            } else
            {
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, toneAmount, vocalToneAmount, 0.5f, formantLerpDuration));
                StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, noiseAmount, breathAmoujnt, 0.4f, formantLerpDuration));
            }

            float currentFormantOneValue = (float)cSoundObj.GetChannel(formantOne); // Fetch current value
            float currentFormantTwoValue = (float)cSoundObj.GetChannel(formantTwo); // Fetch current value
            float currentFormantThreeValue = (float)cSoundObj.GetChannel(formantThree); // Fetch current value
            float currentFormantFourValue = (float)cSoundObj.GetChannel(formantFour); // Fetch current value
            float currentFormantFiveValue = (float)cSoundObj.GetChannel(formantFive); // Fetch current value

            StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, formantOne, currentFormantOneValue, frequencies[0], formantLerpDuration));
            StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, formantTwo, currentFormantTwoValue, frequencies[1], formantLerpDuration));
            StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, formantThree, currentFormantThreeValue, frequencies[2], formantLerpDuration));
            StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, formantFour, currentFormantFourValue, frequencies[3], formantLerpDuration));
            StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, formantFive, currentFormantFiveValue, frequencies[4], formantLerpDuration));
            StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, speakOneShot, true));
        }
    }



    private void OnDestroy()
    {
        // soundEvent.release();
    }

    public void StopSpeaking()
    {
        // soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    private struct PhonemeInfo
    {
        public string phoneme;
        public List<int> frequencies;
        // Other properties can be added if necessary.
    }
}
