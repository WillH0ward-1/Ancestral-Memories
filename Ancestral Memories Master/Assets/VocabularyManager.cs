using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class VocabularyManager : MonoBehaviour
{
    private static readonly object lockObject = new object();
    private static List<string> globalVocabulary = new List<string>();

    public void AddVocabulary(IEnumerable<string> words)
    {
        lock (lockObject)
        {
            globalVocabulary.AddRange(words);
            globalVocabulary = globalVocabulary.Distinct().ToList(); // Removes duplicates
        }
    }

    public void SaveVocabularyToFile()
    {
        lock (lockObject)
        {
            string path = Path.Combine(Application.dataPath, "EveryWord.txt");
            File.WriteAllLines(path, globalVocabulary);
        }
    }
}
