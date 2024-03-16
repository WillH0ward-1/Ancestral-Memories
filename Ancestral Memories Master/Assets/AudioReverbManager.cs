using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioReverbManager : MonoBehaviour
{
    public List<AudioReverbFilter> reverbList;
    public AudioMixer mixer;
    public Player player;

    // Define the send path in the AudioMixer
    private string reverbSendPath = "Reverb"; // This should be the exposed parameter for controlling the volume of your reverb group.

    // Define the minimum and maximum distances for attenuation
    public float minDistance = 1f;
    public float maxDistance = 100;

    public float maxWetReverb = -10000f;
    public float maxDryReverb = 0f;

    private void Awake()
    {
        reverbList = new List<AudioReverbFilter>(GetComponentsInChildren<AudioReverbFilter>());
    }
    void Update()
    {
        if (player != null)
        {
            // Calculate distance between player and this object
            float distance = Vector3.Distance(player.transform.position, transform.position);

            // Normalize the distance within our min and max range
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            float normalizedDistance = (distance - minDistance) / (maxDistance - minDistance);

            // Calculate the send level based on distance (you may adjust this formula to fit your needs)
            // Assuming at min distance, we want no reverb (dry signal), and at max distance, full reverb (wet signal).
            float reverbLevel = Mathf.Lerp(0f, 1f, normalizedDistance); // 0 dB is full volume, -80 dB is virtually silent in Unity's mixer

            // Set the send level to the reverb group. 
            // You might need to adjust how the level is applied depending on your AudioMixer setup.
            foreach (AudioReverbFilter reverb in reverbList)
            {
                reverb.reverbLevel = Mathf.Lerp(maxWetReverb, maxDryReverb, reverbLevel);
            }
        }
    }
}
