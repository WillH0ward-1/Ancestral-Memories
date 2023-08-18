using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine; // Needed for Application.dataPath
using System.Linq;

namespace Qkmaxware.Phonetics
{
    public class IPA
    {
        private Dictionary<string, string> dictionary;
        private HashSet<string> ipaSymbols = new HashSet<string>();

        private static readonly string ipaIndexRawPath = Path.Combine(Application.dataPath, "LanguageGen", "CharResources");

        public IPA()
        {
            this.dictionary = new Dictionary<string, string>();

            string filePath = Path.Combine(Application.dataPath, "CsPhonetics-master", "Phonetics", "data", "CMU.in.IPA.txt");

            foreach (string line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(',', 2);
                if (parts.Length == 2)
                {
                    dictionary[parts[0].Trim()] = parts[1].Trim();
                    ExtractSymbols(parts[1].Trim());
                }
            }

            WriteSymbolsToFile();
            GenerateFormantFile();
        }

        private void ExtractSymbols(string transcription)
        {
            foreach (var character in transcription)
            {
                // Only add characters that are not whitespace to the set
                if (!char.IsWhiteSpace(character))
                {
                    ipaSymbols.Add(character.ToString());
                }
            }
        }


        private void WriteSymbolsToFile()
        {
            string outputFilePath = Path.Combine(ipaIndexRawPath, "IPAindex.txt");
            File.WriteAllLines(outputFilePath, ipaSymbols.OrderBy(s => s));
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

        private Dictionary<string, string[]> formantDictionary = new Dictionary<string, string[]>
        {
            { "a", new[] { "800", "1300", "2800" } },   // Mock values for example
            { "ɑ", new[] { "700", "1150", "2600" } },   // General American 'father'
            { "æ", new[] { "650", "1700", "2400" } },   // General American 'cat'
            { "e", new[] { "400", "2300", "3000" } },   // Close to 'face'
            { "ɛ", new[] { "600", "1800", "2500" } },   // General American 'bet'
            { "ɚ", new[] { "500", "1500", "2500" } },   // R-colored 'nurse', this is a rough estimate
            { "i", new[] { "300", "2400", "3000" } },   // General American 'fleece'
            { "ɪ", new[] { "400", "2200", "2900" } },   // General American 'kit'
            { "o", new[] { "450", "800", "2600" } },    // Close to 'goat'
            { "ɔ", new[] { "500", "1000", "2500" } },   // General American 'thought'
            { "u", new[] { "300", "800", "2400" } },    // General American 'goose'
            { "ʊ", new[] { "400", "900", "2500" } },    // General American 'foot'
            { "ʌ", new[] { "600", "1200", "2400" } }    // General American 'strut'
        };

        public void GenerateFormantFile()
        {
            string formantFilePath = Path.Combine(ipaIndexRawPath, "IPAFormants.txt");

            List<string> lines = new List<string>();
            foreach (var symbol in ipaSymbols)
            {
                if (formantDictionary.ContainsKey(symbol))
                {
                    var line = $"{symbol}: {string.Join(",", formantDictionary[symbol])}";
                    lines.Add(line);
                }
            }
            File.WriteAllLines(formantFilePath, lines);
        }
    }
}
