using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Qkmaxware.Phonetics;
using UnityEngine;

public class VocabularyManager : MonoBehaviour
{
    private static readonly object lockObject = new object();
    private static List<string> globalVocabulary = new List<string>();

    // Raw paths
    public readonly string persistentPath;
    private string everyWordRawPath;
    private string phoneticBreakdownRawPath;
    private string phonemeFormantsRawPath;

    // Reference to the IPA class
    public IPA ipa;

    public string EveryWordPath => everyWordRawPath;
    public string PhoneticBreakdownPath => phoneticBreakdownRawPath;
    public string PhonemeFormantsPath => phonemeFormantsRawPath;

    // Public property for external scripts to access the vocabulary list
    public IReadOnlyList<string> Vocabulary => globalVocabulary;

    private void Awake()
    {
        everyWordRawPath = Path.Combine(Application.persistentDataPath, "EveryWord.txt");
        phoneticBreakdownRawPath = Path.Combine(Application.persistentDataPath, "PhoneticBreakdown.json");
        phonemeFormantsRawPath = Path.Combine(Application.persistentDataPath, "PhonemeFormants.txt");

        ipa = new IPA();  // Initialize the IPA class (or you can set this through Unity Editor)

        LoadPhoneticData();
    }

    public void AddVocabulary(IEnumerable<string> words)
    {
        lock (lockObject)
        {
            globalVocabulary.Clear();
            globalVocabulary.AddRange(words);
            globalVocabulary = globalVocabulary.Distinct().ToList();
        }
    }

    public void SaveVocabularyToFile()
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(EveryWordPath);
            File.WriteAllLines(EveryWordPath, globalVocabulary);
        }
    }

    public void SavePhoneticBreakdownToFile(Dictionary<string, string[]> phoneticData)
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(PhoneticBreakdownPath);
            var jsonData = JsonConvert.SerializeObject(phoneticData, Formatting.Indented);  // Convert Dictionary to JSON string
            File.WriteAllText(PhoneticBreakdownPath, jsonData);  // Write JSON data to file
            SavePhonemeFormantsToFile();
        }
    }

    public void SavePhonemeFormantsToFile()
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(PhonemeFormantsPath);

            // Read and deserialize the JSON file
            string jsonContent = File.ReadAllText(PhoneticBreakdownPath);
            Dictionary<string, List<string>> phoneticData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonContent);

            var lines = new List<string>();

            foreach (var pair in phoneticData)
            {
                string word = pair.Key;
                string phonetic = pair.Value[0];  // Assuming only one phonetic representation per word

                // Placeholder for formant frequency (replace 0 with actual calculation)
                int formantFrequency = 0;

                var line = $"Word: {word} - Phonetic Keys: {phonetic} - Formant Frequency: {formantFrequency}";
                lines.Add(line);
            }

            File.WriteAllLines(PhonemeFormantsPath, lines);
        }
    }


private void EnsureDirectoryExists(string filePath)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
    }

    [SerializeField] private Dictionary<string, string[]> phoneticData;

    private void LoadPhoneticData()
    {
        if (File.Exists(PhoneticBreakdownPath))
        {
            var jsonData = File.ReadAllText(PhoneticBreakdownPath);
            phoneticData = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(jsonData);
        }
        else
        {
            phoneticData = new Dictionary<string, string[]>();
        }
    }

    // Method to count syllables in a sentence
    public int CountSyllablesInSentence(string sentence)
    {
        var words = sentence.Split(new[] { ' ', '\t', '\n', '\r', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        int totalSyllables = 0;

        foreach (var word in words)
        {
            totalSyllables += CountSyllablesInWord(word.ToLower()); // Assuming all words in JSON are lowercase
        }

        return totalSyllables;
    }

    // Method to count syllables in a word based on the phonetic breakdown
    public int CountSyllablesInWord(string word)
    {
        if (phoneticData.ContainsKey(word))
        {
            string phoneticKeys = phoneticData[word][0]; // Assuming the first element is the IPA
            return CountSyllablesInPhonetic(phoneticKeys);
        }

        // Fallback to your previous method if word is not in dictionary
        return FallbackSyllableCount(word);
    }

    // Method to count syllables in a phonetic string
    private int CountSyllablesInPhonetic(string phoneticKeys)
    {
        int count = 0;
        foreach (var phoneme in phoneticKeys.Split(new char[] { 'ˈ', 'ˌ' }))
        {
            if (phoneme.Length > 0)
            {
                count++;
            }
        }

        return count;
    }

    // Fallback method for counting syllables in words not found in the dictionary
    private int FallbackSyllableCount(string word)
    {
        int count = 0;
        foreach (var letter in word)
        {
            if (ipa.vowels.Contains(letter.ToString()))
            {
                count++;
            }
        }

        return count;
    }
}
