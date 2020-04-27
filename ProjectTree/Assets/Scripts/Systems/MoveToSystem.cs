using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

namespace Systems
{
    [AlwaysSynchronizeSystem]
    public class MoveToSystem : JobComponentSystem
    {
        private NavMeshQuery _query;
        // private static NativeMultiHashMap<NativeString32, float3> _positions;
        // private static bool _hasStart;

        protected override void OnCreate()
        {
            base.OnCreate();
            _query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Persistent, 100);
            // _positions = new NativeMultiHashMap<NativeString32, float3>(1024, Allocator.Persistent);
        }

        private static NativeList<float3> FindPath(float3 startPosition, float3 destination,
            NavMeshQuery navMeshQuery)
        {
            var start = navMeshQuery.MapLocation(startPosition, Vector3.one * Random.Range(.5f, 10), 0);
            var end = navMeshQuery.MapLocation(destination, Vector3.one * Random.Range(.5f, 10), 0);
            var status = navMeshQuery.BeginFindPath(start, end);

            navMeshQuery.UpdateFindPath(256, out int performed);

            status = navMeshQuery.EndFindPath(out int polySize);
            var polygonIds = new NativeArray<PolygonId>(polySize + 1, Allocator.Temp);
            navMeshQuery.GetPathResult(polygonIds);
            var positions = new NativeList<float3>(Allocator.Temp);
            // Debug.Log("Start " + startPosition);
            int counter = 0;
            for (int c = 1; c < polySize - 1; c += 2)
            {
                Vector3 left;
                Vector3 right;
                navMeshQuery.GetPortalPoints(polygonIds[c], polygonIds[c + 1], out left, out right);
                float3 position = (left + right) / 2;
                positions.Add(position);
                // RaycastHit hit;
                // if (Physics.Raycast(position, destination - position, out hit))
                // {
                //     if (hit.collider.tag.Equals("Base"))
                //         Debug.Log("\t" + positions[counter]);
                // }

                counter++;
            }

            positions.Add(destination);
            // Debug.Log("\t" + positions[counter]);
            // Debug.LogError("End ");
            return positions;
        }

        // private static float3 GetPosition(NativeString32 key, int counter)
        // {
        //     var enumerator = _positions.GetValuesForKey(key);
        //     enumerator.Reset();
        //     for (int i = 0; i <= counter; i++)
        //     {
        //         enumerator.MoveNext();
        //     }
        //
        //     return enumerator.Current;
        // }
        //
        // private static int PositionsCount(NativeString32 key)
        // {
        //     return _positions.CountValuesForKey(key);
        // }

        private static float Magnitude(float3 vector)
        {
            var magnitude = (math.sqrt(math.pow(vector.x, 2) + math.pow(vector.z, 2)));
            return magnitude;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var query = _query;
            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                    {
                        // if (aiData.state == 0)
                        if (!aiData.changePosition)
                        {
                            var path = FindPath(translation.Value, aiData.finalPosition, query);
                            if (path.Length > 1)
                            {
                                aiData.position = path[0];
                                aiData.changePosition = true;
                            }
                            else
                            {
                                movementData.directionX = 0;
                                movementData.directionY = 0;
                                movementData.directionZ = 0;
                            }
                        }
                        else
                        {
                            var direction = new float3(aiData.position.x - translation.Value.x, 0,
                                aiData.position.z - translation.Value.z);
                            var magnitude = Magnitude(direction);
                            if (magnitude < 1)
                                aiData.changePosition = false;
                            else
                            {
                                direction /= magnitude;
                                movementData.directionX = direction.x;
                                movementData.directionY = 0;
                                movementData.directionZ = direction.z;
                            }
                        }
                    }).Run();
            return default;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _query.Dispose();
            // _positions.Dispose();
        }
    }
}