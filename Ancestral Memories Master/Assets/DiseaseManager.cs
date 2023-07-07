using UnityEngine;

public class Disease
{
    private string[] severities = { "mild", "severe", "fatal", "terminal" };
    public string Severity { get; private set; }
    public bool IsContracted { get; private set; }

    public Disease()
    {
        Severity = "";
        IsContracted = false;
    }

    public void Contract()
    {
        if (!IsContracted)
        {
            int index = Random.Range(0, severities.Length - 1);
            Severity = severities[index];
            Debug.Log("AI has been infected with a " + Severity + " disease!");
            IsContracted = true;
        }
    }

    public float GetDamageMultiplier()
    {
        switch (Severity)
        {
            case "mild":
                return 2f;

            case "severe":
                return 4f;

            case "fatal":
                return 6f;

            case "terminal":
                return 8f;

            default:
                return 0f; // No disease
        }
    }

    public void Heal()
    {
        Debug.Log("AI has miraculously recovered from " + Severity + " disease!");
        IsContracted = false;
    }
}
