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
    protected override void OnUpdate()
    {
        var buffers = GetBufferFromEntity<EnemiesInRange>();
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.WithAll<EnemyAttackPositionComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            var length = buffers[entity].Length;
            var distance = GenerateArray(5, length, 10);
            var positionCount = GenerateArray(10, length, 10);
            var list = GetPositionAround(translation.Value, distance, positionCount);
            for (int i = 0; i < length; i++)
            {
                var enemy = buffers[entity][i].Value;

                if (manager.Exists(enemy))
                {
                    var aiData = manager.GetComponentData<AIData>(enemy);
                    if (!aiData.goToEntity)
                    {
                        aiData.goToEntity = true;
                        aiData.entity = list[i];
                        manager.SetComponentData(enemy, aiData);
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = list[i];
                    }
                }
                else
                {
                    buffers[entity].RemoveAt(i);
                }
            }

            list.Dispose();
        });
    }

    private static NativeList<float> GenerateArray(float toMultiply, int length, int maxEnemies)
    {
        NativeList<float> values = new NativeList<float>(Allocator.TempJob);
        for (int i = 1; i <= (length / maxEnemies) + 1; i++)
        {
            values.Add(toMultiply * i);
        }

        return values;
    }

    private static NativeList<float3> GetPositionAround(float3 startPosition, NativeList<float> ringDistance,
        NativeList<float> ringPositionCount)
    {
        NativeList<float3> positions = new NativeList<float3>(Allocator.TempJob);
        startPosition.y = 0;
        for (int ring = 0; ring < ringPositionCount.Length; ring++)
        {
            var list = GetPositionAround(startPosition, ringDistance[ring], (int) ringPositionCount[ring]);
            positions.AddRange(list);
            list.Dispose();
        }

        ringDistance.Dispose();
        ringPositionCount.Dispose();
        return positions;
    }

    private static NativeList<float3> GetPositionAround(float3 startPosition, float distance, int positionCount)
    {
        NativeList<float3> positions = new NativeList<float3>(Allocator.TempJob);
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