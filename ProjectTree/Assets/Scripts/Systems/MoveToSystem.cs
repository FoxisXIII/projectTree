using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

namespace Systems
{
    [AlwaysSynchronizeSystem]
    public class MoveToSystem : JobComponentSystem
    {
        private NavMeshQuery _query;

        protected override void OnCreate()
        {
            base.OnCreate();
            _query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Persistent, 100);
        }

        private static NativeArray<float3> FindPath(float3 startPosition, float3 destination,
            NavMeshQuery _navMeshQuery)
        {
            var start = _navMeshQuery.MapLocation(startPosition, Vector3.one*Random.Range(.5f,10), 0);
            var end = _navMeshQuery.MapLocation(destination, Vector3.one*Random.Range(.5f,10), 0);
            var status = _navMeshQuery.BeginFindPath(start, end);

            _navMeshQuery.UpdateFindPath(256, out int performed);

            status = _navMeshQuery.EndFindPath(out int polySize);
            var polygonIds = new NativeArray<PolygonId>(polySize, Allocator.Temp);
            _navMeshQuery.GetPathResult(polygonIds);
            var positions = new NativeArray<float3>(polySize, Allocator.Temp);
            Debug.Log("Start");
            int c = 0;
            foreach (var polygon in polygonIds)
            {
                var navMeshLocation = _navMeshQuery.CreateLocation(startPosition, polygon);
                positions[c] = navMeshLocation.position;
                Debug.Log("\t" + positions[c]);
                c++;
            }

            Debug.Log("End ");
            return positions;
        }

        private static float Magnitude(float3 vector)
        {
            var magnitude = (math.sqrt(math.pow(vector.x, 2) + math.pow(vector.z, 2)));
            Debug.Log(magnitude);
            return magnitude;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var query = _query;
            Entities
                .ForEach((ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                {
                    if (!aiData.hasPath)
                    {
                        aiData.hasPath = true;

                        var navMeshPositions = FindPath(translation.Value, new float3(50, 0, 0), query);
                        if (navMeshPositions.Length < 2)
                            aiData.position = new float3(50, 0, 0);
                        else
                            aiData.position = navMeshPositions[1];
                    }
                    else
                    {
                        var direction = new float3(aiData.position.x - translation.Value.x, 0,
                            aiData.position.z - translation.Value.z);
                        var magnitude = Magnitude(direction);
                        if (magnitude < 1)
                            aiData.hasPath = false;
                        else
                        {
                            direction /= magnitude;
                            movementData.directionX = direction.x;
                            movementData.directionZ = direction.z;
                        }
                    }
                }).Run();
            return default;
        }
    }
}