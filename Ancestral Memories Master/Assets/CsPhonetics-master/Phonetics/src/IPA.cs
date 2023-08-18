using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine; // Needed for Application.dataPath

namespace Qkmaxware.Phonetics
{
    public class IPA
    {

        private Dictionary<string, string> dictionary;

        // Default constructor with hardcoded path
        public IPA()
        {
            this.dictionary = new Dictionary<string, string>();

            // Combine the paths to get the correct file path
            string filePath = Path.Combine(Application.dataPath, "CsPhonetics-master", "Phonetics", "data", "CMU.in.IPA.txt");

            foreach (string line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(',', 2);
                if (parts.Length == 2)
                    dictionary[parts[0].Trim()] = parts[1].Trim();
            }
        }

        // Constructor to accept a different dictionary
        public IPA(Dictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        public string EnglishToIPA(string text)
        {
            var builder = new StringBuilder();
            string[] words = Regex.Split(text, @"([\s\p{P}])"); // Split on spaces or punctuation

            foreach (var match in words)
            {
                var lower = match.ToLower();
                if (dictionary.ContainsKey(lower))
                {
                    builder.Append(dictionary[lower]);
                }
                else
                {
                    builder.Append(lower);
                }
            }
            return builder.ToString();
        }
    }
}
