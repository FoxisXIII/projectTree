using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct EnemiesInRange : IBufferElementData
{
    public Entity enemies;
}
