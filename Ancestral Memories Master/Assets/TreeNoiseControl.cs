using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNoiseControl : MonoBehaviour
{
    public Renderer renderer; // The tree's Renderer
    public WeatherControl weatherControl; // Reference to the WeatherControl script
    public float lerpSpeed = 1f; // The speed at which we lerp the noise amount

    private float growthStage; // The current growth stage of the tree

    private void Start()
    {
        // Start the coroutine
        StartCoroutine(UpdateNoiseAmount());
    }

    private IEnumerator UpdateNoiseAmount()
    {
        while (true)
        {
            // Get the current growth value from the shader
            growthStage = renderer.material.GetFloat("_T");

            // Calculate noiseAmount based on windIntensity and growthStage
            float noiseAmount = CalculateNoiseAmount(weatherControl.windStrength, growthStage);

            // Get the current noise amount
            float currentNoiseAmount = renderer.material.GetFloat("_NoiseAmount");

            // Lerp the current noise amount to the calculated noise amount
            currentNoiseAmount = Mathf.Lerp(currentNoiseAmount, noiseAmount, Time.deltaTime * lerpSpeed);

            // Update the noise amount in the shader
            renderer.material.SetFloat("_NoiseAmount", currentNoiseAmount);

            yield return null;
        }
    }

    private float CalculateNoiseAmount(float wind, float growth)
    {
        // Define how wind and growth affect the total noise amount.
        // In this example, we just add wind and growth together,
        // but you should replace this with your own formula.
        // This example also assumes that growth is a value between 0 and 1,
        // with 0 indicating no growth and 1 indicating full growth.
        return wind * (1 - growth);
    }
}

