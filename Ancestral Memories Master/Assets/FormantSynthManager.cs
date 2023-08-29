using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Qkmaxware.Phonetics;
using UnityEngine;
using static Qkmaxware.Phonetics.IPA;

public class FormantSynthManager : MonoBehaviour
{
    public static FormantSynthManager Instance;

    public Dictionary<string, PhonemeInfo> PhoneticData { get; private set; } = new Dictionary<string, PhonemeInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPhoneticData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadPhoneticData()
    {
        string ipaIndexPath = Path.Combine(Application.dataPath, "LanguageGen", "CharResources", "IPAindex.json");

        if (!File.Exists(ipaIndexPath))
        {
            Debug.LogError("IPAindex.json file not found.");
            return;
        }

        string jsonContent = File.ReadAllText(ipaIndexPath);
        Dictionary<string, IPASymbol> deserializedIpaSymbols = JsonConvert.DeserializeObject<Dictionary<string, IPASymbol>>(jsonContent);

        foreach (var entry in deserializedIpaSymbols)
        {
            string character = entry.Key;
            IPASymbol ipaSymbol = entry.Value;

            PhonemeInfo phonemeInfo = new PhonemeInfo
            {
                phoneme = ipaSymbol.Symbol,
                frequencies = new List<int>
                {
                    ipaSymbol.F1,
                    ipaSymbol.F2,
                    ipaSymbol.F3,
                    ipaSymbol.F4,
                    ipaSymbol.F5
                }
            };

            PhoneticData[character] = phonemeInfo;
        }
    }

    public struct PhonemeInfo
    {
        public string phoneme;
        public List<int> frequencies;
    }
}
