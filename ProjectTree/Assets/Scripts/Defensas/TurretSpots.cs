using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretSpots : MonoBehaviour
{
    public GameObject[] spots;

    public Vector3 GetNearestpointOnGrid(float3 position)
    {
        float3 closestPoint = float3.zero;
        float minDistance = 10000;
        for (int x = 0; x < spots.Length; x++)
        {
            float currentDistance = math.distance(spots[x].transform.position, position);
            if (minDistance > currentDistance)
            {
                closestPoint = spots[x].transform.position;
                minDistance = currentDistance;
            }
        }

        if (minDistance >= 10000)
            return closestPoint;
        return float3.zero;
    }

    public void EnableParticles()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            spots[i].GetComponent<CreatingSpot>().ActivateParticles();
        }
    }

    public void DisableParticles()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            spots[i].GetComponent<CreatingSpot>().StopParticles();
        }
    }
}
