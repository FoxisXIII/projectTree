using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class AnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((ref AnimationData animationData, ref AIData aiData, in Entity entity) =>
            {
                if (animationData._animationType == 0)
                {
                    if (aiData.canFly)
                    {
                        RotateHelixes(manager, animationData, deltaTime);
                    }
                }
                else if (animationData._animationType == 1)
                {
                    if (aiData.canFly)
                    {
                        RotateHelixes(manager, animationData, deltaTime);
                    }
                    else
                    {
                        RotateTyres(manager, animationData, deltaTime,animationData.rotationSpeed);
                    }
                }
            }).WithoutBurst().Run();
    }

    private void RotateTyres(EntityManager manager, AnimationData animationData, float deltaTime, float rotationSpeed)
    {
        var rotationHelixL = manager.GetComponentData<Rotation>(animationData.helixL);
        rotationHelixL.Value = math.mul(rotationHelixL.Value, quaternion.RotateX(rotationSpeed * deltaTime));
        manager.SetComponentData(animationData.helixL, rotationHelixL);
        var rotationHelixR = manager.GetComponentData<Rotation>(animationData.helixR);
        rotationHelixR.Value = math.mul(rotationHelixR.Value, quaternion.RotateX(rotationSpeed * deltaTime));
        manager.SetComponentData(animationData.helixR, rotationHelixR);
        
        var rotationHullHelixL = manager.GetComponentData<Rotation>(animationData.hullHelixL);
        rotationHullHelixL.Value = math.mul(rotationHullHelixL.Value, quaternion.RotateX(rotationSpeed * deltaTime));
        manager.SetComponentData(animationData.hullHelixL, rotationHullHelixL);
        var rotationHullHelixR = manager.GetComponentData<Rotation>(animationData.hullHelixR);
        rotationHullHelixR.Value = math.mul(rotationHullHelixR.Value, quaternion.RotateX(rotationSpeed * deltaTime));
        manager.SetComponentData(animationData.hullHelixR, rotationHullHelixR);
    }

    private static void RotateHelixes(EntityManager manager, AnimationData animationData, float deltaTime)
    {
        var rotationHelixL = manager.GetComponentData<Rotation>(animationData.helixL);
        rotationHelixL.Value = math.mul(rotationHelixL.Value, quaternion.RotateZ(90f * deltaTime));
        manager.SetComponentData(animationData.helixL, rotationHelixL);
        var rotationHelixR = manager.GetComponentData<Rotation>(animationData.helixR);
        rotationHelixR.Value = math.mul(rotationHelixR.Value, quaternion.RotateZ(90f * deltaTime));
        manager.SetComponentData(animationData.helixR, rotationHelixR);
    }
}