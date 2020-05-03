using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    public float directionX;
    public float directionY;
    public float directionZ;
    public float speed;
}