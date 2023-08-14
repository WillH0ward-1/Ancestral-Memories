using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LanguageGenerator : MonoBehaviour
{
    public enum Consonants { P, B, T, D, K, G, F, V, S, Z, SH, ZH, M, N, NG }
    public enum Vowels { A, E, I, O, U }

    private Dictionary<string, string> EnglishToNeanderthalDictionary = new Dictionary<string, string>();

    private void Start()
    {
        string path = Path.Combine(Application.dataPath, "EveryWord.txt");
        string[] englishWords = File.ReadAllLines(path);

        foreach (string englishWord in englishWords)
        {
            string trimmedEnglishWord = englishWord.Trim();
            string fictionalWord = CreateWord(trimmedEnglishWord.Length / 2);
            EnglishToNeanderthalDictionary[trimmedEnglishWord] = fictionalWord;
        }
    }

    public string TranslateToNeanderthal(string englishInput)
    {
        string[] tokens = TokenizeEnglish(englishInput.ToLower());
        string neanderthalOutput = "";

        foreach (string token in tokens)
        {
            string neanderthalWord;
            if (EnglishToNeanderthalDictionary.TryGetValue(token, out neanderthalWord))
            {
                neanderthalOutput += ApplyGrammarRules(token, neanderthalWord) + " ";
            }
            else
            {
                neanderthalOutput += CreateWord(token.Length / 2) + " ";
            }
        }

        return neanderthalOutput.Trim();
    }

    private string ApplyGrammarRules(string englishWord, string neanderthalWord)
    {
        if (englishWord.EndsWith("s") && englishWord != "is")
        {
            neanderthalWord += "ru";
        }
        return neanderthalWord;
    }

    private string CreateWord(int syllableCount)
    {
        string word = "";
        for (int i = 0; i < syllableCount; i++)
        {
            Consonants consonant = (Consonants)Random.Range(0, System.Enum.GetNames(typeof(Consonants)).Length);
            Vowels vowel = (Vowels)Random.Range(0, System.Enum.GetNames(typeof(Vowels)).Length);
            word += CreateSyllable(consonant, vowel);
        }
        return word;
    }

    private string CreateSyllable(Consonants consonant, Vowels vowel)
    {
        return consonant.ToString().ToLower() + vowel.ToString().ToLower();
    }

    private string[] TokenizeEnglish(string englishInput)
    {
        return englishInput.Split(' ');
    }

    public void OnTranslateButtonPressed(string englishInput)
    {
        string translatedText = TranslateToNeanderthal(englishInput);
        Debug.Log(translatedText);
    }
}
