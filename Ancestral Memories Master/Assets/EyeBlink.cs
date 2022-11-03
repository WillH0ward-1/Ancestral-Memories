using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlink : MonoBehaviour
{
    [SerializeField] private Renderer[] eyeRenderers;

    public GameObject eyeLids;

    public float minBlinkSpeed = 0.1f;
    public float maxBlinkSpeed = 0.2f;

    public float min = 1;
    public float max = 4;

    private float randomRetrigger;

    private void Awake()
    {
        randomRetrigger = Random.Range(min, max);
        eyeRenderers = eyeLids.GetComponentsInChildren<Renderer>();
        Invoke(nameof(EyeOpen), randomRetrigger);
    }

    void Blink()
    {
        randomRetrigger = Random.Range(min, max);
        CloseEyes(eyeLids);
        Invoke(nameof(EyeOpen), randomRetrigger);
        return;
    }

    void EyeOpen()
    {
        randomRetrigger = Random.Range(minBlinkSpeed, maxBlinkSpeed);
        OpenEyes(eyeLids);
        Invoke(nameof(Blink), randomRetrigger);
        return;
    }

    public void CloseEyes(GameObject state)
    {

        MeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }

    }

    public void OpenEyes(GameObject state)
    {
        MeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }
}
