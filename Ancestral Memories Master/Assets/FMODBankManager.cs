using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class FMODBankManager : MonoBehaviour
{
    public StudioBankLoader Player;
    public StudioBankLoader PlayerInstruments;
    public StudioBankLoader Music;
    public StudioBankLoader Weather;
    public StudioBankLoader Lightning;
    public StudioBankLoader Cave;
    public StudioBankLoader Actions;
    public StudioBankLoader Tree;
    public StudioBankLoader UI;
    public StudioBankLoader Animals;
    public StudioBankLoader Footsteps;
    public StudioBankLoader Fire;
    public StudioBankLoader NPC;

    private void Awake()
    {
        //LoadBank(Player);
    }


    StudioBankLoader studioBankloader;

    public void LoadBank(StudioBankLoader bank)
    {
        bank.Load();

        return;
    }

    public void UnloadBank(StudioBankLoader bankLoader)
    {
        bankLoader.Unload();
    }

}
