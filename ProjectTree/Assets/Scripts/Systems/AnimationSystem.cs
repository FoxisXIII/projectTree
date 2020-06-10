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
                    else
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
                        RotateHulls(manager, animationData, deltaTime, 45);
                        RotateHelixes(manager, animationData, deltaTime);
                    }
                }
            }).WithoutBurst().Run();
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