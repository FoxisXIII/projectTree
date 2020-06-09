using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretSpots : MonoBehaviour
{
    private Transform[] spots;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spots[i] = transform.GetChild(i);
        }
    }
    
    public Vector3 GetNearestpointOnGrid(float3 position)
    {
        float3 closestPoint = float3.zero;
        float minDistance = 10000;
        for (int x = 0; x < spots.Length; x++)
        {
            float currentDistance = math.distance(spots[x].position, position);
            if (minDistance > currentDistance)
            {
                closestPoint = spots[x].position;
                minDistance = currentDistance;
            }
        }

        if (minDistance >= 10000)
            return closestPoint;
        return float3.zero;
    }
}
