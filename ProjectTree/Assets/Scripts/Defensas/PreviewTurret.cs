using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTurret : MonoBehaviour
{
    public Material material;
    public Color canBePlaced, canNotBePlaced;
    public LayerMask layerMask;
    public List<Transform> validPoints;

    public bool isValidPosition()
    {
        int length = validPoints.Count;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (i == j) continue;

                Ray ray = new Ray(validPoints[i].position, validPoints[j].position - validPoints[i].position);
                if (Physics.Raycast(ray, Vector3.Distance(validPoints[i].position, validPoints[j].position),
                    layerMask.value))
                    return false;
            }
        }

        return true;
    }
}