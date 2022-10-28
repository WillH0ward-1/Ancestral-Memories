using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] GameObject EntryPrefab;

    List<RadialMenuEntry> Entries;

    // Start is called before the first frame update
    void Start()
    {
        Entries = new List<RadialMenuEntry>();
    }

    void AddEntry(string pLabel)
    {
        GameObject entry = Instantiate(EntryPrefab, transform);
        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetLabel(pLabel);

        Entries.Add(rme);
    }
    // Update is called once per frame
    public void Open()
    {
        for (int i = 0; i < 5; i++)
        {
            AddEntry("Button" + i.ToString());
        }
    }
}
