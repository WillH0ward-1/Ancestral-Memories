using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FormantSynthesizer : MonoBehaviour
{
    private CsoundUnity csoundUnity;
    private Dictionary<string, PhonemeInfo> phoneticData = new Dictionary<string, PhonemeInfo>();

    private void Awake()
    {
        csoundUnity = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        LoadPhoneticData();
        ProcessNLTK phonemeProcessor = new ProcessNLTK();
    }

    private void LoadPhoneticData()
    {
        string phoneticFilePath = Path.Combine(Application.dataPath, "LanguageGen/CharResources/PhoneticTranscriptions.txt");

        if (!File.Exists(phoneticFilePath))
        {
            Debug.LogError("PhoneticTranscriptions.txt file not found.");
            return;
        }

        string[] phoneticLines = File.ReadAllLines(phoneticFilePath);

        foreach (string line in phoneticLines)
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                string word = parts[0];
                string[] phonemeData = parts[1].Split(',');

                PhonemeInfo phonemeInfo = new PhonemeInfo
                {
                    phoneme = word,
                    // Load frequencies, syllable count, stress pattern, etc.
                };

                phoneticData[word] = phonemeInfo;
            }
        }
    }

    private List<int> GetFormantFrequenciesForPhoneme(string phoneme)
    {
        List<int> frequencies = new List<int>();

        if (ProcessNLTK.PhonemeMapping.PhonemeFormants.ContainsKey(phoneme))
        {
            ProcessNLTK.PhonemeFormant phonemeFormant = ProcessNLTK.PhonemeMapping.PhonemeFormants[phoneme];

            // For simplicity, assuming the first set of frequencies from the FormantFrequencies list
            if (phonemeFormant.FormantFrequencies.Count > 0)
            {
                frequencies = phonemeFormant.FormantFrequencies[0];
            }
        }

        return frequencies;
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

    private int GetSyllableCount(string[] phonemeData)
    {
        // Implement this method to get the syllable count from the phoneme data
        return 0;
    }

    private List<string> GetStressPattern(string[] phonemeData)
    {
        // Implement this method to get the stress pattern from the phoneme data
        return new List<string>();
    }

    public void Speak(string word)
    {
        if (!phoneticData.ContainsKey(word))
        {
            Debug.LogWarning($"No phonetic data found for word: {word}");
            return;
        }

        PhonemeInfo phonemeInfo = phoneticData[word];
        foreach (string phoneme in phonemeInfo.phonemes)
        {
            if (ProcessNLTK.PhonemeMapping.PhonemeFormants.ContainsKey(phoneme))
            {
                List<List<int>> formantFrequencies = ProcessNLTK.PhonemeMapping.PhonemeFormants[phoneme].FormantFrequencies;
                SendFrequenciesToCsoundInstrument(formantFrequencies[0]); // Sending first set of formant frequencies
            }
        }
    }

    private struct PhonemeInfo
    {
        public string phoneme;
        public List<string> phonemes;
        public List<int> frequencies;
        public int syllableCount;
        public List<string> stressPattern;
        // Add other fields as needed
    }
}
