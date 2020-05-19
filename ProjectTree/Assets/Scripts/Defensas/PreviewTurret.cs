using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTurret : MonoBehaviour
{
    public Material material;
    public Color canBePlaced, canNotBePlaced;
    public LayerMask groundLayerMask;
    public List<Transform> validPoints;
    public float distanceToGround;

    public bool isValidPosition()
    {
        int length = validPoints.Count;
        for (int i = 0; i < length; i++)
        {
            Ray groundRay = new Ray(validPoints[i].position, Vector3.down);
            if (Physics.Raycast(groundRay, distanceToGround, groundLayerMask.value))
            {
                return true;
            }
        }
        return false;
    }
}