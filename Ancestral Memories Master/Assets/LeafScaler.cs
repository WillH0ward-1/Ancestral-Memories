using System.Collections;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class LeafScaler : MonoBehaviour
{
    public ProceduralTree proceduralTree;
    public PTGrowing pTGrowing;

    public float minGrowthScale = 0f; // The minimum scale for the leaves
    public float maxGrowthScale = 1f; // The maximum scale for the leaves
    [SerializeField] private float growthScale;

    [SerializeField] private float currentScale = 0f;

    [SerializeField] private bool isLerping = false;
    [SerializeField] private float lerpStart;
    [SerializeField] private float lerpEnd;
    public float lerpduration;
    [SerializeField] private float lerpTimeElapsed;

    private void OnEnable()
    {
        pTGrowing = transform.GetComponentInChildren<PTGrowing>();
    }

    public void LerpScale(float start, float end, float duration)
    {
        isLerping = true;
        lerpStart = start;
        lerpEnd = end;
        lerpduration = duration;
        lerpTimeElapsed = 0;
    }

    private void Update()
    {
        if (proceduralTree != null)
        {
            if (isLerping)
            {
                if (lerpTimeElapsed < lerpduration)
                {
                    currentScale = Mathf.Lerp(lerpStart, lerpEnd, lerpTimeElapsed / lerpduration);
                    SetLeafScale(currentScale);
                    lerpTimeElapsed += Time.deltaTime;
                }
                else
                {
                    currentScale = lerpEnd;
                    SetLeafScale(currentScale);
                    isLerping = false;
                }
            }
        }
    }

    private Matrix4x4[] originalMatrices;  // Keep track of the original scale

    public void RecordOriginalMatrices()
    {
        if (proceduralTree != null && proceduralTree.Matrices != null)
        {
            originalMatrices = new Matrix4x4[proceduralTree.Matrices.Length];
            for (int i = 0; i < proceduralTree.Matrices.Length; i++)
            {
                originalMatrices[i] = proceduralTree.Matrices[i];
            }
        }
    }

    public void SetLeafScale(float scale)
    {
        if (proceduralTree != null && originalMatrices != null)
        {
            for (int i = 0; i < originalMatrices.Length; i++)
            {
                // Extract the original translation from the matrix
                Vector3 originalTranslation = originalMatrices[i].GetColumn(3);

                // Get the stored rotation
                Quaternion originalRotation = proceduralTree.LeafRotations[i];

                // Create a new scale vector
                Vector3 scaleVector = new Vector3(scale, scale, scale);

                // Construct a new matrix with the original rotation, original translation, and new scale
                proceduralTree.Matrices[i] = Matrix4x4.TRS(originalTranslation, originalRotation, scaleVector);
            }
        }
    }













}
