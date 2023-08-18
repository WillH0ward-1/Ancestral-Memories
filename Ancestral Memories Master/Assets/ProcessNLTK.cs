using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Debug = UnityEngine.Debug;

public class ProcessNLTK
{
    /*
    private string scriptPath = Path.Combine("Assets", "PythonScripts", "languageToolKit.py");
    private string libPath = Path.Combine("Assets", "Plugins", "Lib");

    public struct PhonemeFormant
    {
        public string Phoneme;
        public List<List<int>> FormantFrequencies;

        public PhonemeFormant(string phoneme, List<List<int>> formantFrequencies)
        {
            Phoneme = phoneme;
            FormantFrequencies = formantFrequencies;
        }
    }

    public static class PhonemeMapping
    {
        public static Dictionary<string, PhonemeFormant> PhonemeFormants = new Dictionary<string, PhonemeFormant>();
        // Rest of PhonemeMapping class
    }

    public void SetupNLTK(string everyWordFilePath)
    {
        var engine = Python.CreateEngine();

        ICollection<string> searchPaths = engine.GetSearchPaths();
        searchPaths.Add(libPath);

        engine.SetSearchPaths(searchPaths);

        ScriptScope scope = engine.CreateScope();
        scope.SetVariable("NTLK", this);
       
        // Execute the Python script
        try
        {
            engine.ExecuteFile(libPath);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred while executing the Python script: " + e.Message);

            // Print additional information from Python's error trace
            if (e.Data.Contains("IronPythonStackTrace"))
            {
                Debug.LogError("Python error trace: " + e.Data["IronPythonStackTrace"]);
            }

            return;
        }

        // Check if NLTK was imported successfully
        if (engine.GetSysModule().GetVariable("nltk") != null)
        {
            Debug.Log("NLTK imported successfully.");
        }
        else
        {
            Debug.LogError("NLTK was not imported successfully.");
        }

        LoadPhonemeData(everyWordFilePath); // Pass the EveryWord.txt path
        LoadAdditionalData(); // Load emotion and other additional data
                              // Other NLTK setup calls
    }



    private void LoadPhonemeData(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                string phoneme = parts[0];
                List<List<int>> formantFrequencies = ParseFormantData(parts[1]);
                PhonemeMapping.PhonemeFormants[phoneme] = new PhonemeFormant(phoneme, formantFrequencies);
            }
        }
    }

    public static Dictionary<string, List<int>> PhonemeFormantFrequencies = new Dictionary<string, List<int>>();

    // Method to precompute and store formant frequencies for all phonemes
    public static void PrecomputeFormantFrequencies()
    {

    }

    private void LoadAdditionalData()
    {
        LoadEmotionData("Assets/LanguageGen/CharResources/EmotionData.txt");
        // Load other additional data files if needed
    }

    private void LoadEmotionData(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                string word = parts[0];
                string emotion = parts[1];
                // Store emotion data in a dictionary or data structure as needed
            }
        }
    }

    private List<List<int>> ParseFormantData(string data)
    {
        List<List<int>> formantFrequencies = new List<List<int>>();
        string[] formantSets = data.Split(';');
        foreach (string formantSet in formantSets)
        {
            string[] frequencies = formantSet.Split('_');
            List<int> frequenciesList = new List<int>();
            foreach (string freqStr in frequencies)
            {
                if (int.TryParse(freqStr, out int freq))
                {
                    frequenciesList.Add(freq);
                }
            }
            formantFrequencies.Add(frequenciesList);
        }
        return formantFrequencies;
    }
*/
}
