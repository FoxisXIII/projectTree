using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TrapDamageSystem))]
public class TrapDeathSystem : JobComponentSystem
{
    private static quaternion openedRot;
    private static quaternion leftClosedRot;
    private static quaternion rightClosedRot;
    private static float lerpTime;
    private static double startTime;

    private EndSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
        ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        openedRot = new quaternion(-0.5f, -0.5f, -0.5f, 0.5f);
        leftClosedRot = new quaternion(-0.7071068f, -0.7071068f, 0, 0);
        rightClosedRot = new quaternion(0, 0, -0.7071068f, 0.7071068f);
        lerpTime = 25;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityCommandBuffer ecb = this.ecb.CreateCommandBuffer();
        float deltaTime = Time.DeltaTime;
        double time = Time.ElapsedTime;

        Entities.ForEach((ref TrapComponent trapComponent, ref Entity entity, in AnimationData animationData) =>
        {
            if(trapComponent.times==0)
            {
                ecb.AddComponent<Dead>(entity);
            }
            if (!trapComponent.cankill)
            {
                if (trapComponent.Recover == 0)
                    CloseTrap(manager, animationData, time);
                if (trapComponent.Recover >= 2)
                {
                    trapComponent.cankill = true;
                }
                else
                {
                    trapComponent.Recover += deltaTime;
                    OpenTrap(manager, animationData, time);
                }
            }
        }).WithoutBurst().Run();

        return default;
    }

    private static void CloseTrap(EntityManager manager, in AnimationData animationData, double time)
    {
        var rotationHelixL = manager.GetComponentData<Rotation>(animationData.helixL);
        rotationHelixL.Value = leftClosedRot;
        manager.SetComponentData(animationData.helixL, rotationHelixL);
        var rotationHelixR = manager.GetComponentData<Rotation>(animationData.helixR);
        rotationHelixR.Value = rightClosedRot;
        manager.SetComponentData(animationData.helixR, rotationHelixR);
        startTime = time;
    }

    private static void OpenTrap(EntityManager manager, in AnimationData animationData, double time)
    {
        float timeProgressed = (float) (time - startTime) / lerpTime;
        var rotationHelixL = manager.GetComponentData<Rotation>(animationData.helixL);
        rotationHelixL.Value = Quaternion.Lerp(rotationHelixL.Value, openedRot, timeProgressed);
        manager.SetComponentData(animationData.helixL, rotationHelixL);
        var rotationHelixR = manager.GetComponentData<Rotation>(animationData.helixR);
        rotationHelixR.Value = Quaternion.Lerp(rotationHelixR.Value, openedRot, timeProgressed);
        manager.SetComponentData(animationData.helixR, rotationHelixR);
    }
}