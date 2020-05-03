using Unity.Entities;
using System;

public struct ParentEntity : IComponentData
{
    public Entity parent;
}
