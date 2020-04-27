using Unity.Entities;
using System;

[Serializable]
public struct TimeToLive : IComponentData
{
    public float Value;
}
