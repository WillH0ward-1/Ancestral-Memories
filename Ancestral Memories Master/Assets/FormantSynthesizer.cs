using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FormantSynthesizer : MonoBehaviour
{
    private CsoundUnity csoundUnity;
    private Dictionary<string, PhonemeInfo> phoneticData = new Dictionary<string, PhonemeInfo>();

    private VocabularyManager VocabularyManager;

    private void Awake()
    {
        csoundUnity = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        LoadPhoneticData();
    }

    private void LoadPhoneticData()
    {
        string phoneticFilePath = Path.Combine(Application.dataPath, VocabularyManager.PhoneticBreakdownPath);

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
        List<int> frequncies = new List<int>();

        SendFrequenciesToCsoundInstrument(frequncies);
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
