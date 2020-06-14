using System;
using System.Collections.Generic;
using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

[UpdateBefore(typeof(MoveToSystem))]
public class HordeSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var currentEnemies = GameController.GetInstance().CurrentEnemies;
        var maxWaveEnemies = GameController.GetInstance().MaxWaveEnemies;
        Entities
            .ForEach(
                (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                    ref Entity entity, ref DynamicBuffer<CollisionEnemy> collisionEnemies) =>
                {
                    if (aiData.horde && currentEnemies == maxWaveEnemies)
                    {
                        aiData.hordeMove = true;
                    }
                }).Run();
        return default;
    }
}