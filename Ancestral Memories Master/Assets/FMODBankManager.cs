using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class FMODBankManager : MonoBehaviour
{
    //[FMODUnity.BankRef] public List<string> banks = new List<string>();

    private enum Banks
    {
        Master,
        Player,
        PlayerInstruments,
        Music,
        Weather,
        Lightning,
        Cave,
        Actions,
        Tree,
        UI,
        Animals,
        Footsteps,
        Fire,
        NPC

    }

    [SerializeField] private List<StudioBankLoader> fmodBanks;

    /*
    private void LoadAllBanks()
    {
        foreach (string bank in banks)
        {
            RuntimeManager.LoadBank(bank);
        }
    }
    */

    private void Awake()
    {
        LoadBank("Master");
        LoadBank("Music");
        LoadBank("Player");
        LoadBank("Weather");
        LoadBank("Tree");
        LoadBank("Footsteps");
        LoadBank("Fire");
        LoadBank("Animals");
        LoadBank("UI");

    }

    public void LoadBank(string bank)
    {
        RuntimeManager.LoadBank(bank);
    }

    public void UnloadBank(string bank)
    {
        RuntimeManager.UnloadBank(bank);
    }

}
