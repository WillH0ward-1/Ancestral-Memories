using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Qkmaxware.Phonetics;
using UnityEngine;
using static Qkmaxware.Phonetics.IPA;
using FMODUnity;
using FMOD.Studio;

public class FormantSynthesizer : MonoBehaviour
{
    private Dictionary<string, PhonemeInfo> phoneticData = new Dictionary<string, PhonemeInfo>();

    private VocabularyManager VocabularyManager;
    private IPA ipaInstance; // Assuming you have an IPA class, instantiate or get its reference

    [SerializeField] private EventReference soundEventRef;
    EventInstance soundEvent;

    //private string eventPath = "event:/NPC/FormantSynthesizer"; // Replace with the actual path of your event

    private void Start()
    {
        ipaInstance = new IPA(); // Create a new instance of the IPA class or get its reference if it's a singleton
        LoadPhoneticData();

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

    public void Speak(string word)
    {
        string ipaRepresentation = ipaInstance.EnglishToIPA(word);
        List<int[]> allFormants = ipaInstance.GetFormants(ipaRepresentation);

        foreach (int[] phonemeFormants in allFormants)
        {
            SendFreq(phonemeFormants);
        }
    }

    private void SendFreq(int[] frequencies)
    {
        if (frequencies.Length >= 3)
        {
            // Create an instance of the FMOD event
            soundEvent = RuntimeManager.CreateInstance(soundEventRef);

            soundEvent.setParameterByName("Freq1", frequencies[0]);
            soundEvent.setParameterByName("Freq2", frequencies[1]);
            soundEvent.setParameterByName("Freq3", frequencies[2]);

            soundEvent.start();
            soundEvent.release();
        }
    }


    private void OnDestroy()
    {
        soundEvent.release();
    }

    public void StopSpeaking()
    {
        soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    private struct PhonemeInfo
    {
        public string phoneme;
        public List<int> frequencies;
        // Other properties can be added if necessary.
    }
}
