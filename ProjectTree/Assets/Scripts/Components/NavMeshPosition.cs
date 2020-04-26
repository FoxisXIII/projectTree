using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.AI;

public struct NavMeshPosition : IBufferElementData
{
    public float3 position;
    public PolygonId polygon;
}