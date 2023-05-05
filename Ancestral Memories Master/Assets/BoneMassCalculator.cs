using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BoneMassCalculator : MonoBehaviour
{
    public float totalMass = 70f;
    public Transform rootBone;
    public bool calculateOnAwake = true;

    public List<Transform> boneList = new List<Transform>();

    private void Awake()
    {
        if (calculateOnAwake)
        {
            CalculateBoneMasses();
        }
    }

    public void CalculateBoneMasses()
    {
        if (rootBone == null)
        {
            Debug.LogError("Root bone not set.");
            return;
        }

        boneList.Clear();
        foreach (Transform child in rootBone.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("Hips") || child.name.Contains("Spine") || child.name.Contains("Arm") ||
                child.name.Contains("ForeArm") || child.name.Contains("Hand") || child.name.Contains("UpLeg") ||
                child.name.Contains("Leg") || child.name.Contains("Foot") || child.name.Contains("Shoulder") ||
                child.name.Contains("Neck") || child.name.Contains("Head"))
            {
                // Use the instance ID to ensure that each bone is unique
                if (!boneList.Exists(bone => bone.GetInstanceID() == child.GetInstanceID()))
                {
                    boneList.Add(child);
                }
            }
        }


        var totalBoneMass = 0f;

        // Calculate total mass and mass for each bone
        foreach (var bone in boneList)
        {
            float boneMass = 0f;
            var boneName = bone.name;
            if (boneName.Contains("Hips") || boneName.Contains("Spine"))
            {
                boneMass = 1.2f * totalMass;
            }
            else if (boneName.Contains("Arm") || boneName.Contains("ForeArm"))
            {
                boneMass = 0.05f * totalMass;
            }
            else if (boneName.Contains("Hand"))
            {
                boneMass = 0.01f * totalMass;
            }
            else if (boneName.Contains("UpLeg"))
            {
                boneMass = 0.1f * totalMass;
            }
            else if (boneName.Contains("Leg"))
            {
                boneMass = 0.05f * totalMass;
            }
            else if (boneName.Contains("Foot"))
            {
                boneMass = 0.02f * totalMass;
            }
            else if (boneName.Contains("Shoulder") || boneName.Contains("Neck"))
            {
                boneMass = 0.02f * totalMass;
            }
            else if (boneName.Contains("Head"))
            {
                boneMass = 0.05f * totalMass;
            }

            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = boneMass;
            }
            else
            {
                Debug.LogWarning($"No Rigidbody found for bone: {boneName}");
            }

            totalBoneMass += boneMass;
        }

        // Adjust root bone mass if necessary
        var rootRigidbody = rootBone.GetComponent<Rigidbody>();
        if (rootRigidbody != null)
        {
            var massDelta = totalMass - totalBoneMass;
            rootRigidbody.mass += massDelta;
        }
        else
        {
            Debug.LogWarning("No Rigidbody found for root bone.");
        }
    }
}
