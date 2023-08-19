using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Qkmaxware.Phonetics;
using UnityEngine;
using static Qkmaxware.Phonetics.IPA;

public class FormantSynthesizer : MonoBehaviour
{
    private CsoundUnity csoundUnity;
    private Dictionary<string, PhonemeInfo> phoneticData = new Dictionary<string, PhonemeInfo>();

    private VocabularyManager VocabularyManager;
    private IPA ipaInstance; // Assuming you have an IPA class, instantiate or get its reference

    private void Awake()
    {
        csoundUnity = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        LoadPhoneticData();
        LoadFormantData();
        ipaInstance = new IPA(); // Create a new instance of the IPA class or get its reference if it's a singleton

    }

    private void LoadPhoneticData()
    {
        string ipaIndexPath = Path.Combine(Application.dataPath, "LanguageGen", "CharResources", "IPAindex.json");

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
            },
                // You can populate other properties such as syllableCount, stressPattern, etc., 
                // if they're available in IPASymbol or another source.
            };

            phoneticData[character] = phonemeInfo;
        }
    }

    private Dictionary<string, List<int>> formantFrequencies = new Dictionary<string, List<int>>();

    private void LoadFormantData()
    {
        string formantFilePath = Path.Combine(Application.dataPath, "LanguageGen", "CharResources", "IPAFormants.txt");

        if (!File.Exists(formantFilePath))
        {
            Debug.LogError("IPAFormants.txt file not found.");
            return;
        }

        string[] lines = File.ReadAllLines(formantFilePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length == 2)
            {
                string phoneme = parts[0].Trim();
                List<int> frequencies = parts[1].Split(',').Select(int.Parse).ToList();
                formantFrequencies[phoneme] = frequencies;
            }
        }
    }

    public void Speak(string letter)
    {
        string ipaRepresentation = ipaInstance.EnglishToIPA(letter); // This method should return the IPA representation for the word

        if (phoneticData.TryGetValue(ipaRepresentation, out PhonemeInfo phonemeInfo))
        {
            foreach (string phoneme in phonemeInfo.phonemes)
            {
                if (formantFrequencies.TryGetValue(phoneme, out List<int> freqs))
                {
                    SendFrequenciesToCsoundInstrument(freqs);
                }
                else
                {
                    Debug.LogWarning($"No formant data found for phoneme: {phoneme}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No phonetic data found for word: {letter}");
        }
    }

    private void SendFrequenciesToCsoundInstrument(List<int> frequencies)
    {
        if (frequencies.Count >= 3 && csoundUnity != null)
        {
            csoundUnity.SetChannel("iFreq1", frequencies[0]);
            csoundUnity.SetChannel("iFreq2", frequencies[1]);
            csoundUnity.SetChannel("iFreq3", frequencies[2]);

            // Construct a score event for instrument 1 with a duration of 1 second
            string scoreEvent = $"i 1 0 1 {frequencies[0]} {frequencies[1]} {frequencies[2]}";
            csoundUnity.SendScoreEvent(scoreEvent);
        }
    }

    private struct PhonemeInfo
    {
        public string phoneme;
        public List<string> phonemes;
        public List<int> frequencies;
        public int syllableCount;
        public List<string> stressPattern;
    }
}
