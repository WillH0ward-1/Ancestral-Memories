using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDictionary : MonoBehaviour
{
    public List<string> maleNames = new List<string>();
    public List<string> femaleNames = new List<string>();

    // Male phonemes
    private string[] maleStartPhonemes = new string[] { "Bar", "El", "Kan", "Mel", "Zan" };
    private string[] maleMiddlePhonemes = new string[] { "ra", "os", "im", "ur" };
    private string[] maleEndPhonemes = new string[] { "am", "esh", "on", "as" };

    // Female phonemes
    private string[] femaleStartPhonemes = new string[] { "Ana", "Eli", "Mara", "Sara", "Lana" };
    private string[] femaleMiddlePhonemes = new string[] { "na", "li", "ra", "la" };
    private string[] femaleEndPhonemes = new string[] { "ah", "elle", "ia", "is" };

    public static NameDictionary Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetRandomMaleName()
    {
        return GenerateUniqueName(maleNames, maleStartPhonemes, maleMiddlePhonemes, maleEndPhonemes);
    }

    public string GetRandomFemaleName()
    {
        return GenerateUniqueName(femaleNames, femaleStartPhonemes, femaleMiddlePhonemes, femaleEndPhonemes);
    }

    string GenerateUniqueName(List<string> nameList, string[] startPhonemes, string[] middlePhonemes, string[] endPhonemes)
    {
        HashSet<string> usedPhonemes = new HashSet<string>();
        string name;
        int attempts = 0;
        do
        {
            name = GenerateName(startPhonemes, middlePhonemes, endPhonemes, usedPhonemes);
            attempts++;
        }
        while ((maleNames.Contains(name) || femaleNames.Contains(name)) && attempts < 10);

        if (attempts >= 10)
        {
            Debug.LogWarning("Failed to generate a unique name after multiple attempts.");
            return null;
        }

        // Add the new unique name to the respective list
        nameList.Add(name);
        return name;
    }

    string GenerateName(string[] startPhonemes, string[] middlePhonemes, string[] endPhonemes, HashSet<string> usedPhonemes)
    {
        string name = ChoosePhoneme(startPhonemes, usedPhonemes);
        int middlePhonemeCount = Random.Range(1, 3); // Choose 1 or 2 middle phonemes

        for (int i = 0; i < middlePhonemeCount; i++)
        {
            name += ChoosePhoneme(middlePhonemes, usedPhonemes);
        }

        name += ChoosePhoneme(endPhonemes, usedPhonemes);

        return name;
    }

    string ChoosePhoneme(string[] phonemeSet, HashSet<string> usedPhonemes)
    {
        string chosenPhoneme;
        do
        {
            chosenPhoneme = phonemeSet[Random.Range(0, phonemeSet.Length)];
        }
        while (usedPhonemes.Contains(chosenPhoneme));

        usedPhonemes.Add(chosenPhoneme);
        return chosenPhoneme;
    }
}
