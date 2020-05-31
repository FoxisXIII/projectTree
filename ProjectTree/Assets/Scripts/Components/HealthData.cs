using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct HealthData : IComponentData
{
    public int value;
    public int maxValue;
}