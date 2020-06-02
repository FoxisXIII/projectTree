using Unity.Entities;

[GenerateAuthoringComponent]
public struct AnimationData : IComponentData
{
    public int _animationType;
    public Entity helixL, helixR;
    public Entity hullHelixL, hullHelixR;
}