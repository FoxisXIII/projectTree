using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(DeathSystem))]
public class ResolveDamageSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
        ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = this.ecb.CreateCommandBuffer();
        Entities.WithNone<Dead>().ForEach(
            (Entity e, ref DynamicBuffer<Damage> damageBuffer, ref HealthData hp) =>
            {
                for (int i = 0; i < damageBuffer.Length; i++)
                {
                    hp.value -= damageBuffer[i].Value;

                    if (hp.value <= 0)
                    {
                        hp.value = 0;
                        ecb.AddComponent<Dead>(e);
                        break;
                    }
                }
                damageBuffer.Clear();
            }).Run();

        return default;
    }
}
