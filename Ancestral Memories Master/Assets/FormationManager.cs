using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager
{
    public enum FormationType
    {
        None,
        Circle,
        Square,
        // Add any other formation types you want here
    }

    public static List<Vector3> GetPositions(Vector3 center, FormationType type, int count, float size)
    {
        List<Vector3> positions = new List<Vector3>();

        switch (type)
        {
            case FormationType.None:
                for (int i = 0; i < count; i++)
                {
                    positions.Add(center);
                }
                break;
            case FormationType.Circle:
                for (int i = 0; i < count; i++)
                {
                    float angle = i * 360f / count;
                    Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * size;
                    positions.Add(center + offset);
                }
                break;
            case FormationType.Square:
                int sideCount = Mathf.CeilToInt(Mathf.Sqrt(count));
                for (int i = 0; i < sideCount; i++)
                {
                    for (int j = 0; j < sideCount; j++)
                    {
                        if (positions.Count >= count) break;

                        Vector3 offset = new Vector3((i - sideCount / 2) * size, 0, (j - sideCount / 2) * size);
                        positions.Add(center + offset);
                    }
                }
                break;
                // Add cases for any other formation types here
        }
        return positions;
    }
}

