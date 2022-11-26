using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchControllerZombie : MonoBehaviour
{
    //public values
    public float minColChange = 700;
    public float maxColChange = 1400;
    public float minIntensChange = 400;
    public float maxIntensChange = 800;
    public float minIntensity = 0.2f;
    public float maxIntensity = 0.4f;
    public Color darkColor;
    public Color brightColor;


    //init values
    Color oldColor;
    Color color;
    private GameObject[] torches;

    void Start()
    {
        torches = GameObject.FindGameObjectsWithTag("TorchLightZombie");
        color = new Color(
            // Change first and third color to change bounds of colorvariation, 
            // then paste down to other block
            Random.Range(darkColor.r, brightColor.r),
            Random.Range(darkColor.g, brightColor.g),
            Random.Range(darkColor.b, brightColor.b)
        );
        
        if(torches.Length != 0){
            oldColor = torches[0].GetComponent<Light>().color;
        }
    }

    void FixedUpdate()
    {
        //Iterate through all torchlights
        foreach (GameObject torch in torches)
        {
            //Calculate Color
            if (
                torch.GetComponent<TorchVariables>().counterColor
                <= torch.GetComponent<TorchVariables>().durationColor
            )
            {
                torch.GetComponent<TorchVariables>().counterColor += Time.deltaTime * 1000;

                torch.GetComponent<Light>().color = Color.Lerp(
                    oldColor,
                    color,
                    torch.GetComponent<TorchVariables>().counterColor
                        / torch.GetComponent<TorchVariables>().durationColor
                );
            }
            else
            {
                torch.GetComponent<TorchVariables>().counterColor = 0f;
                oldColor = torch.GetComponent<Light>().color;
                color = new Color(
                    // Replace the folowing three lines
                    Random.Range(darkColor.r, brightColor.r),
                    Random.Range(darkColor.g, brightColor.g),
                    Random.Range(darkColor.b, brightColor.b)
                );
                torch.GetComponent<TorchVariables>().durationColor = Random.Range(
                    minColChange,
                    maxColChange
                );
            }

            //Calculate Intensity
            if (
                torch.GetComponent<TorchVariables>().counterIntens
                <= torch.GetComponent<TorchVariables>().durationIntens
            )
            {
                torch.GetComponent<TorchVariables>().counterIntens += Time.deltaTime * 1000;

                torch.GetComponent<Light>().intensity = Mathf.Lerp(
                    torch.GetComponent<TorchVariables>().oldIntensity,
                    torch.GetComponent<TorchVariables>().intensity,
                    torch.GetComponent<TorchVariables>().counterIntens
                        / torch.GetComponent<TorchVariables>().durationIntens
                );
            }
            else
            {
                torch.GetComponent<TorchVariables>().counterIntens = 0f;
                torch.GetComponent<TorchVariables>().oldIntensity = torch
                    .GetComponent<TorchVariables>()
                    .intensity;
                torch.GetComponent<TorchVariables>().intensity = Random.Range(
                    minIntensity,
                    maxIntensity
                );

                torch.GetComponent<TorchVariables>().durationIntens = Random.Range(
                    minIntensChange,
                    maxIntensChange
                );
            }
        }
    }
}
