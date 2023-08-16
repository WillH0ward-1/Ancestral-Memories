using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FormantSynthesizer : MonoBehaviour
{
    private CsoundUnity csoundUnity;

    void Awake()
    {
        csoundUnity = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        phoneticData = LoadPhoneticData();
    }

    public struct PhonemeFormant
    {
        public string Phoneme;
        public List<int> Frequencies;

        public PhonemeFormant(string phoneme, List<int> frequencies)
        {
            Phoneme = phoneme;
            Frequencies = frequencies;
        }
    }

    public static class PhonemeMapping
    {
        public static Dictionary<string, PhonemeFormant> PhonemeFormants = new Dictionary<string, PhonemeFormant>
        {
            { "iː", new PhonemeFormant("iː", new List<int>{ 270, 2290, 3010 }) },
            { "ɪ", new PhonemeFormant("ɪ", new List<int>{ 390, 1990, 2550 }) },
            { "e", new PhonemeFormant("e", new List<int>{ 530, 1840, 2480 }) },
            { "æ", new PhonemeFormant("æ", new List<int>{ 660, 1720, 2410 }) },
            { "ɑː", new PhonemeFormant("ɑː", new List<int>{ 730, 1090, 2440 }) },
            { "ɒ", new PhonemeFormant("ɒ", new List<int>{ 570, 840, 2410 }) },
            { "ɔː", new PhonemeFormant("ɔː", new List<int>{ 490, 670, 2390 }) },
            { "ʊ", new PhonemeFormant("ʊ", new List<int>{ 440, 1020, 2240 }) },
            { "uː", new PhonemeFormant("uː", new List<int>{ 300, 870, 2240 }) },
            { "eɪ", new PhonemeFormant("eɪ", new List<int>{ 400, 1800, 2400 }) },
            { "aɪ", new PhonemeFormant("aɪ", new List<int>{ 450, 1700, 2400 }) },
            { "ɔɪ", new PhonemeFormant("ɔɪ", new List<int>{ 490, 1050, 2390 }) },
            { "aʊ", new PhonemeFormant("aʊ", new List<int>{ 570, 840, 2350 }) },
            { "oʊ", new PhonemeFormant("oʊ", new List<int>{ 440, 800, 2400 }) },
            { "ɪə", new PhonemeFormant("ɪə", new List<int>{ 430, 1650, 2400 }) },
            { "eə", new PhonemeFormant("eə", new List<int>{ 500, 1370, 2400 }) },
            { "ʊə", new PhonemeFormant("ʊə", new List<int>{ 440, 1350, 2300 }) },
            { "m", new PhonemeFormant("m", new List<int>{ 235, 910, 2350 }) },
            { "n", new PhonemeFormant("n", new List<int>{ 260, 2300, 3450 }) },
            { "ŋ", new PhonemeFormant("ŋ", new List<int>{ 290, 1250, 2100 }) },
            { "f", new PhonemeFormant("f", new List<int>{ 390, 1800, 2550 }) },
            { "v", new PhonemeFormant("v", new List<int>{ 580, 1700, 2550 }) },
            { "θ", new PhonemeFormant("θ", new List<int>{ 650, 1850, 2400 }) },  // 'th' as in "thing"
            { "ð", new PhonemeFormant("ð", new List<int>{ 640, 1740, 2400 }) },  // 'th' as in "that"
            { "s", new PhonemeFormant("s", new List<int>{ 810, 2150, 2450 }) },
            { "z", new PhonemeFormant("z", new List<int>{ 900, 2250, 2450 }) },
            { "ʃ", new PhonemeFormant("ʃ", new List<int>{ 750, 1800, 2450 }) },  // 'sh' as in "she"
            { "ʒ", new PhonemeFormant("ʒ", new List<int>{ 850, 1900, 2500 }) },  // like the 's' in "measure"
            { "h", new PhonemeFormant("h", new List<int>{ 480, 1350, 2400 }) },
            { "ʧ", new PhonemeFormant("ʧ", new List<int>{ 600, 2300, 3400 }) },  // 'ch' as in "chat"
            { "ʤ", new PhonemeFormant("ʤ", new List<int>{ 750, 2500, 3450 }) },  // 'j' as in "judge"
            { "p", new PhonemeFormant("p", new List<int>{ 250, 1100, 2500 }) },
            { "b", new PhonemeFormant("b", new List<int>{ 300, 950, 2300 }) },
            { "t", new PhonemeFormant("t", new List<int>{ 400, 1900, 2800 }) },
            { "d", new PhonemeFormant("d", new List<int>{ 450, 1700, 2600 }) },
            { "k", new PhonemeFormant("k", new List<int>{ 340, 850, 2300 }) },
            { "g", new PhonemeFormant("g", new List<int>{ 390, 750, 2100 }) },
            { "r", new PhonemeFormant("r", new List<int>{ 360, 1550, 2600 }) },  // 'r' can be tricky and varies a lot by accent
            { "l", new PhonemeFormant("l", new List<int>{ 450, 1450, 2500 }) },
            { "j", new PhonemeFormant("j", new List<int>{ 400, 1900, 2900 }) },  // 'y' as in "yes"
            { "w", new PhonemeFormant("w", new List<int>{ 370, 850, 2400 })
            }
        };

        private static string PathToEveryWordFile = Path.Combine(Application.dataPath, "EveryWord.txt");

        public static void SetPhoneticVocabulary()
        {
            if (!File.Exists(PathToEveryWordFile))
            {
                Debug.LogError("EveryWord.txt file not found.");
                return;
            }

            var allWords = File.ReadAllLines(PathToEveryWordFile);
            foreach (string word in allWords)
            {
             //   string phoneme = GetPhonemeForWord(word);  // This function needs to be implemented
             //   if (!string.IsNullOrEmpty(phoneme) && !PhonemeFormants.ContainsKey(phoneme))
                {
                    // Assuming you have a way to get the formant frequencies for new phonemes
                   // List<int> frequencies = GetFormantFrequenciesForPhoneme(phoneme);  // This function needs to be implemented

               //     if (frequencies != null)
                    {
               //         PhonemeFormants[phoneme] = new PhonemeFormant(phoneme, frequencies);
                    }
                }
            }
        }
    }

    private Dictionary<string, List<string>> LoadPhoneticData()
    {
        string[] phoneticLines = File.ReadAllLines(Path.Combine(Application.dataPath, "PhoneticTranscriptions.txt"));

        Dictionary<string, List<string>> wordPhonemes = new Dictionary<string, List<string>>();
        foreach (string line in phoneticLines)
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                wordPhonemes[parts[0]] = new List<string>(parts[1].Split(','));
            }
        }

        return wordPhonemes;
    }

    private Dictionary<string, List<string>> phoneticData;

    public void Speak(string word)
    {
        if (!phoneticData.ContainsKey(word))
        {
            Debug.LogWarning($"No phonetic data found for word: {word}");
            return;
        }

        List<string> phonemes = phoneticData[word];
        foreach (string phoneme in phonemes)
        {
            if (PhonemeMapping.PhonemeFormants.ContainsKey(phoneme))
            {
                List<int> frequencies = PhonemeMapping.PhonemeFormants[phoneme].Frequencies;
                SendFrequenciesToCsoundInstrument(frequencies);
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
}
