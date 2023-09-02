using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public string EveryWordPath => everyWordRawPath;
    public string PhoneticBreakdownPath => phoneticBreakdownRawPath;
    public string PhonemeFormantsPath => phonemeFormantsRawPath;

    // Public property for external scripts to access the vocabulary list
    public IReadOnlyList<string> Vocabulary => globalVocabulary;

    private void Awake()
    {
        everyWordRawPath = Path.Combine(Application.dataPath, "Resources", "Dialogue", "EveryWord.txt");
        phoneticBreakdownRawPath = Path.Combine(Application.dataPath, "Resources", "Dialogue", "PhoneticBreakdown.txt");
        phonemeFormantsRawPath = Path.Combine(Application.dataPath, "Resources", "Dialogue", "PhonemeFormants.txt");
    }

    public void AddVocabulary(IEnumerable<string> words)
    {
        lock (lockObject)
        {
            globalVocabulary.Clear(); // Clear the existing vocabulary first
            globalVocabulary.AddRange(words);
            globalVocabulary = globalVocabulary.Distinct().ToList(); // Removes duplicates
        }
    }

    public void SaveVocabularyToFile()
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(EveryWordPath); // Ensure the directory exists
            File.WriteAllLines(EveryWordPath, globalVocabulary);
        }
    }

    public void SavePhoneticBreakdownToFile(Dictionary<string, string[]> phoneticData)
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(PhoneticBreakdownPath); // Ensure the directory exists

            var lines = new List<string>();
            foreach (var pair in phoneticData)
            {
                var line = $"Word: {pair.Key} - Phonetic Keys: {string.Join(", ", pair.Value)}";
                lines.Add(line);
            }

            File.WriteAllLines(PhoneticBreakdownPath, lines);

            SavePhonemeFormantsToFile();
        }
    }

    public void SavePhonemeFormantsToFile()
    {
        lock (lockObject)
        {
            EnsureDirectoryExists(PhonemeFormantsPath); // Ensure the directory exists

            var phoneticBreakdownLines = File.ReadAllLines(PhoneticBreakdownPath);
            var lines = new List<string>();

            foreach (var breakdownLine in phoneticBreakdownLines)
            {
                var wordStart = breakdownLine.IndexOf("Word: ") + "Word: ".Length;
                var wordEnd = breakdownLine.IndexOf(" - Phonetic Keys:");
                var word = breakdownLine.Substring(wordStart, wordEnd - wordStart);

                var phoneticStart = breakdownLine.IndexOf("Phonetic Keys: ") + "Phonetic Keys: ".Length;
                var phonetics = breakdownLine.Substring(phoneticStart).Split(new[] { ", " }, StringSplitOptions.None);

                // Placeholder for formant frequency (replace 0 with the actual calculated value later)
                int formantFrequency = 0;

                var line = $"Word: {word} - Phonetic Keys: {string.Join(", ", phonetics)} - Formant Frequency: {formantFrequency}";
                lines.Add(line);
            }

            File.WriteAllLines(PhonemeFormantsPath, lines);
        }
    }


    // Helper method to ensure directory exists for a given file path
    private void EnsureDirectoryExists(string filePath)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
    }
}
