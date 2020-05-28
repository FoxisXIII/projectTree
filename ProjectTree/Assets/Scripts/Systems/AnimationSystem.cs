using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class AnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var materials = GameController.GetInstance().getMaterials();
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Entities
            .WithStructuralChanges()
            .ForEach((ref AnimationData animationData, ref AIData aiData, in Entity entity) =>
        {
            if (animationData._animationType != animationData._lastAnimationType)
            {
                animationData._lastAnimationType = animationData._animationType;
                if (aiData.canFly)
                {
                    var renderMesh = manager.GetSharedComponentData<RenderMesh>(entity);
                    renderMesh.material = materials["Dron"][animationData._animationType];
                    manager.SetSharedComponentData<RenderMesh>(entity, renderMesh);
                }
                else
                {
                    var renderMesh = manager.GetSharedComponentData<RenderMesh>(entity);
                    renderMesh.material = materials["Tank"][animationData._animationType];
                    manager.SetSharedComponentData<RenderMesh>(entity, renderMesh);
                }
            }
        }).WithoutBurst().Run();
    }
}