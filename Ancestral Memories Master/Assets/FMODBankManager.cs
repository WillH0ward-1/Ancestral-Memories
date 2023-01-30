using UnityEngine;
using FMODUnity;

public class FMODBankManager : MonoBehaviour
{
    private void Awake()
    {
        LoadBank("Master.strings");
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
        RuntimeManager.LoadBank(bank, true);
    }

    public void UnloadBank(string bank)
    {
        RuntimeManager.UnloadBank(bank);
    }

}
