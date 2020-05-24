using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct EnemyPosition : IBufferElementData
{
    public float3 position;
}