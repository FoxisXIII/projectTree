using Unity.Entities;
using System;

[Serializable]
[GenerateAuthoringComponent]
public struct TimeToLive : IComponentData
{
    public float Value;
}
