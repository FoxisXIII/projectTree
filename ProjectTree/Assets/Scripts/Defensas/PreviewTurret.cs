using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTurret : MonoBehaviour
{
    public Material material;
    public Color canBePlaced, canNotBePlaced;
    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;
    public List<Transform> validPoints;
    public float distanceToGround;

    public bool isValidPosition()
    {
        int length = validPoints.Count;
        for (int i = 0; i < length; i++)
        {
            Ray groundRay = new Ray(validPoints[i].position, Vector3.down);
            if (!Physics.Raycast(groundRay, distanceToGround, groundLayerMask.value))
            {
                return false;
            }
        }
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if(i!=j)
                {
                    Ray groundRay = new Ray(validPoints[i].position, validPoints[j].position);
                    if (Physics.Raycast(groundRay, Vector3.Distance(validPoints[i].position, validPoints[j].position),
                        obstacleLayerMask))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}