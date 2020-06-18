using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AnimationData : IComponentData
{
    [HideInInspector] public int _animationType;
    public Entity helixL, helixR;
    public Entity hullHelixL, hullHelixR;
    [HideInInspector] public float rotationSpeed;
}