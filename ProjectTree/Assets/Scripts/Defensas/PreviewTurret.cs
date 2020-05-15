using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTurret : MonoBehaviour
{
    public Material material;
    public Color canBePlaced, canNotBePlaced;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    public List<Transform> validPoints;

    public bool isValidPosition()
    {
        int length = validPoints.Count;
        for (int i = 0; i < length; i++)
        {
            Ray groundRay = new Ray(validPoints[i].position, Vector3.down);
            if (Physics.Raycast(groundRay, 0.5f, groundLayerMask.value))
            {
                for (int j = 0; j < length; j++)
                {
                    if (i == j) continue;
                
                    Ray wallRay = new Ray(validPoints[i].position, validPoints[j].position-validPoints[j].position);
                    Debug.DrawRay(wallRay.origin, wallRay.direction);
                    if (!Physics.Raycast(wallRay, Vector3.Distance(validPoints[i].position, validPoints[j].position), wallLayerMask.value))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}