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
                        RotateHulls(manager, animationData, deltaTime, 0);
                        RotateHelixes(manager, animationData, deltaTime);
                    }
                }
                else if (animationData._animationType == 1)
                {
                    if (aiData.canFly)
                    {
                        RotateHulls(manager, animationData, deltaTime, 45);
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
        Debug.Log(rotationHelixL.Value.value.z);
        rotationHelixL.Value = math.mul(rotationHelixL.Value, quaternion.RotateZ(90f * deltaTime));
        manager.SetComponentData(animationData.helixL, rotationHelixL);
        var rotationHelixR = manager.GetComponentData<Rotation>(animationData.helixR);
        rotationHelixR.Value = math.mul(rotationHelixR.Value, quaternion.RotateZ(90f * deltaTime));
        manager.SetComponentData(animationData.helixR, rotationHelixR);
    }

    private static void RotateHulls(EntityManager manager, AnimationData animationData, float deltaTime, float rotation)
    {
        // var rotationHelixL = manager.GetComponentData<Rotation>(animationData.hullHelixL);
        // rotationHelixL.Value = quaternion.RotateX(rotation * deltaTime);
        // manager.SetComponentData(animationData.helixL, rotationHelixL);
        // var rotationHelixR = manager.GetComponentData<Rotation>(animationData.hullHelixR);
        // rotationHelixR.Value = quaternion.RotateX(rotation * deltaTime);
        // manager.SetComponentData(animationData.helixR, rotationHelixR);
    }
}