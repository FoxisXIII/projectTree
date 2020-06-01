using Unity.Entities;

[GenerateAuthoringComponent]
    public struct ParentComponent:IComponentData
    {
        public Entity parent;
    }
