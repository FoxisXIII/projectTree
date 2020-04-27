using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AIData : IComponentData
{
    public int state;

    public float attackDistance;

    public bool changePosition;
    public float3 position;
    public float3 finalPosition;
}