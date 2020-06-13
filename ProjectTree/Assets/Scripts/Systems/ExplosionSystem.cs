using FMOD;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Video;

public class ExplosionSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;


    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        var ecb = ecbSystem.CreateCommandBuffer();

        Entities.ForEach((ref ExplosionComponent explosionComponent, ref Entity entity) =>
        {
            if (explosionComponent.explode)
                ecb.DestroyEntity(entity);
            
            if (explosionComponent.timer < explosionComponent.ttl)
            {
                explosionComponent.timer += deltaTime;
            }
            else
            {
                explosionComponent.explode = true;
                //SoundManager.GetInstance().PlayOneShotSound(paths.BombPath.ToString(), position.Value);
            }
        }).Run();
        return default;
    }
}