using Unity.Entities;

[GenerateAuthoringComponent]
public struct AnimationData : IComponentData
{
    public int _animationType;
    public int _lastAnimationType;
}