using System;
using System.Collections.Generic;
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

        private static NativeList<EnemyPosition> FindPath(float3 startPosition, float3 destination, float3 direction,
            NavMeshQuery navMeshQuery)

        {
            var start = navMeshQuery.MapLocation(startPosition, Vector3.one * 50, 0);
            var end = navMeshQuery.MapLocation(destination, Vector3.one * 50, 0);
            var status = navMeshQuery.BeginFindPath(start, end);

            navMeshQuery.UpdateFindPath(256, out int performed);

            status = navMeshQuery.EndFindPath(out int polySize);
            var polygonIds = new NativeArray<PolygonId>(polySize + 1, Allocator.Temp);
            navMeshQuery.GetPathResult(polygonIds);
            var positions = new NativeList<EnemyPosition>(Allocator.Temp);
            // Debug.Log("Start " + startPosition);
            int counter = 0;
            for (int c = 0; c < polySize; c += 1)
            {
                float3 position = navMeshQuery.CreateLocation(startPosition, polygonIds[c]).position;

                RaycastHit leftHit, rightHit;
                Ray leftRay = new Ray(position, -direction);
                Ray rightRay = new Ray(position, direction);

                float3 right, left;
                if (Physics.Raycast(leftRay, out leftHit, 20))
                    left = leftHit.point;
                else
                {
                    left = position - direction * 20;
                }

                if (Physics.Raycast(rightRay, out rightHit, 20))
                    right = rightHit.point;
                else
                {
                    right = position + direction * 20;
                }

                if (right.x == left.x)
                    position.z = Random.Range(left.z, right.z);
                else if (right.z == left.z)
                    position.x = Random.Range(left.x, right.x);

                // Debug.Log(position);
                positions.Add(new EnemyPosition() {position = position});
            }

            // Debug.LogError(destination);
            positions.Add(new EnemyPosition() {position = destination});

            return positions;
        }


        private static float Magnitude(float3 vector)
        {
            var magnitude = math.sqrt(math.pow(vector.x, 2) + math.pow(vector.z, 2));
            return magnitude;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var query = _query;
            var buffers = GetBufferFromEntity<EnemyPosition>();
            float3 playerPosition = GameController.GetInstance().Player.transform.position;

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                        ref Entity entity) =>
                    {
                        if (math.distance(translation.Value, playerPosition) >= aiData.chaseDistance)
                        {
                            if (aiData.hasToInitialize)
                            {
                                buffers[entity].AddRange(FindPath(translation.Value, aiData.finalPosition,
                                    aiData.direction,
                                    query));
                                aiData.hasToInitialize = false;
                            }
                            else if (!aiData.changePosition)
                            {
                                var direction = new float3(translation.Value.x - aiData.finalPosition.x, 0,
                                    translation.Value.z - aiData.finalPosition.z);
                                if (Magnitude(direction) > 5 && aiData.counter < buffers[entity].Length)
                                {
                                    aiData.counter++;
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
                                var position = buffers[entity][aiData.counter].position;
                                var direction = new float3(position.x - translation.Value.x, 0,
                                    position.z - translation.Value.z);
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
                        }
                        else if (aiData.canAttackPlayer)
                        {
                            var direction = new float3(playerPosition.x - translation.Value.x, 0,
                                playerPosition.z - translation.Value.z);
                            var magnitude = Magnitude(direction);
                            if (magnitude < 1)
                            {
                                movementData.directionX = 0;
                                movementData.directionY = 0;
                                movementData.directionZ = 0;
                            }
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