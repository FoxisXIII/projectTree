using System.Collections.Generic;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[UpdateBefore(typeof(MoveToSystem))]
public class AttackPositionSystem : ComponentSystem
{
    private List<float3> list;

    protected override void OnUpdate()
    {
        var buffers = GetBufferFromEntity<EnemiesInRange>();
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.WithAll<EnemyAttackPositionComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            var length = buffers[entity].Length;

            list = GetPositionAround(translation.Value, 5, 15);

            for (int i = 0; i < length; i++)
            {
                var enemy = buffers[entity][i].Value;
        
                if (manager.Exists(enemy))
                {
                    var aiData = manager.GetComponentData<AIData>(enemy);
                    aiData.goToEntity = true;
                    aiData.entity = list[i%list.Count];
                    manager.SetComponentData(enemy, aiData);
                }
                else
                {
                    buffers[entity].RemoveAt(i);
                    length = buffers[entity].Length;
                }
            }
        });
    }

    private static List<float> GeneratePositionCountArray(float toMultiply, int length, int maxEnemies)
    {
        List<float> values = new List<float>();
        for (int i = 1; i <= (length / maxEnemies) + 1; i++)
        {
            values.Add(toMultiply * i);
        }

        return values;
    }

    private static List<float> GenerateArray(float toMultiply, int length, int maxEnemies)
    {
        List<float> values = new List<float>();
        for (int i = 1; i <= (length / maxEnemies) + 1; i++)
        {
            values.Add(toMultiply + (i * 1.5f));
        }

        return values;
    }

    private static List<float3> GetPositionAround(float3 startPosition, List<float> ringDistance,
        List<float> ringPositionCount)
    {
        List<float3> positions = new List<float3>();
        startPosition.y = 0;
        for (int ring = 0; ring < ringPositionCount.Count; ring++)
        {
            var list = GetPositionAround(startPosition, ringDistance[ring], (int) ringPositionCount[ring]);
            positions.AddRange(list);
        }

        return positions;
    }

    private static List<float3> GetPositionAround(float3 startPosition, float distance, int positionCount)
    {
        List<float3> positions = new List<float3>();
        startPosition.y = 0;
        for (int i = 1; i <= positionCount; i++)
        {
            int angle = i * (360 / positionCount);
            float3 dir = ApplyRotationToVector(new float3(0, 1, 0), angle);
            dir = new float3(dir.x, dir.z, dir.y);
            float3 position = startPosition + dir * distance;
            positions.Add(position);
        }

        return positions;
    }

    private static float3 ApplyRotationToVector(float3 vector, int angle)
    {
        return Quaternion.Euler(0, 0, angle) * vector;
    }
}