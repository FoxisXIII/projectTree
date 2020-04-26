using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AIData : IComponentData
{
    public NativeString64 state;

    public bool hasPath;
    public float3 position;
}