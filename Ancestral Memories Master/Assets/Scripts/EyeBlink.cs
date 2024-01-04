using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlink : MonoBehaviour
{
    [SerializeField] private Renderer[] eyeRenderers;
    public GameObject root;

    public float minBlinkSpeed = 0.1f;
    public float maxBlinkSpeed = 0.2f;
    public float min = 1;
    public float max = 4;

    private float randomRetrigger;
    private bool allowBlinking = true; // Flag to control blinking

    private void Awake()
    {
        eyeRenderers = root.GetComponentsInChildren<Renderer>();
        StartBlinking();
    }

    public void StartBlinking()
    {
        if (!allowBlinking) return;

        randomRetrigger = Random.Range(min, max);
        Invoke(nameof(EyeOpen), randomRetrigger);
    }

    void Blink()
    {
        if (!allowBlinking) return;

        randomRetrigger = Random.Range(min, max);
        CloseEyes();
        Invoke(nameof(EyeOpen), randomRetrigger);
    }

    void EyeOpen()
    {
        if (!allowBlinking) return;

        randomRetrigger = Random.Range(minBlinkSpeed, maxBlinkSpeed);
        OpenEyes();
        Invoke(nameof(Blink), randomRetrigger);
    }

    public void CloseEyes()
    {
        // Enabling the renderers to 'close' the eyes
        foreach (Renderer renderer in eyeRenderers)
        {
            renderer.enabled = true;
        }
    }

    public void OpenEyes()
    {
        // Disabling the renderers to 'open' the eyes
        foreach (Renderer renderer in eyeRenderers)
        {
            renderer.enabled = false;
        }
    }

    public void StopBlinking()
    {
        if (!allowBlinking) return;

        allowBlinking = false;
        CancelInvoke(nameof(Blink));
        CancelInvoke(nameof(EyeOpen));
        CloseEyes();
    }
}
