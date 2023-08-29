using System.Collections.Generic;
using Qkmaxware.Phonetics;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class FormantSynthesizer : MonoBehaviour
{
    private IPA ipaInstance;

    [SerializeField] private EventReference soundEventRef;
    EventInstance soundEvent;

    private void Start()
    {
        ipaInstance = new IPA();  // Create a new instance or get its reference if it's a singleton
    }

    public void Speak(string letter)
    {
        string ipaRepresentation = ipaInstance.EnglishToIPA(letter);

        if (FormantSynthManager.Instance.PhoneticData.TryGetValue(ipaRepresentation, out FormantSynthManager.PhonemeInfo phonemeInfo))
        {
            SendFreq(phonemeInfo.frequencies);
        }
        else
        {
            Debug.LogWarning($"No phonetic data found for word: {letter}");
        }
    }

    private void SendFreq(List<int> frequencies)
    {
        if (frequencies.Count >= 3)
        {
            soundEvent = RuntimeManager.CreateInstance(soundEventRef);

            soundEvent.setParameterByName("Freq1", frequencies[0]);
            soundEvent.setParameterByName("Freq2", frequencies[1]);
            soundEvent.setParameterByName("Freq3", frequencies[2]);

            soundEvent.start();
            soundEvent.release();
        }
    }

    private void OnDestroy()
    {
        soundEvent.release();
    }

    public void StopSpeaking()
    {
        soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
