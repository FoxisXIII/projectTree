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

        protected override void OnCreate()
        {
            base.OnCreate();
            _query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Persistent, 100);
        }

        private static float3 FindPath(float3 startPosition, float3 destination,
            NavMeshQuery navMeshQuery, float3 offset)

        {
            destination += offset;
            var start = navMeshQuery.MapLocation(startPosition, Vector3.one * 10, 0);
            var end = navMeshQuery.MapLocation(destination, Vector3.one * 10, 0);
            var status = navMeshQuery.BeginFindPath(start, end);

            navMeshQuery.UpdateFindPath(512, out int performed);

            status = navMeshQuery.EndFindPath(out int polySize);
            var polygonIds = new NativeArray<PolygonId>(polySize + 1, Allocator.Temp);
            navMeshQuery.GetPathResult(polygonIds);

            var apexIndex = 0;

            var startPolyWorldToLocal = navMeshQuery.PolygonWorldToLocalMatrix(polygonIds[0]);

            var apex = startPolyWorldToLocal.MultiplyPoint(startPosition);
            var left = new Vector3(0, 0, 0); // Vector3.zero accesses a static readonly which does not work in burst yet
            var right = new Vector3(0, 0, 0);
            var leftIndex = -1;
            var rightIndex = -1;
            var n = 0;
            int maxStraightPath = polySize;
            NativeArray<NavMeshLocation> straightPath =
                new NativeArray<NavMeshLocation>(maxStraightPath, Allocator.Temp);


            for (var i = 1; i <= polySize; ++i)
            {
                var polyWorldToLocal = navMeshQuery.PolygonWorldToLocalMatrix(polygonIds[apexIndex]);

                Vector3 vl, vr;
                if (i == polySize)
                {
                    vl = vr = polyWorldToLocal.MultiplyPoint(destination);
                }
                else
                {
                    var success = navMeshQuery.GetPortalPoints(polygonIds[i - 1], polygonIds[i], out vl, out vr);

                    vl = polyWorldToLocal.MultiplyPoint(vl);
                    vr = polyWorldToLocal.MultiplyPoint(vr);
                }

                vl -= apex;
                vr -= apex;

                if (Perp2D(vl, vr) < 0)
                    Swap(ref vl, ref vr);
                // Terminate funnel by turning
                if (Perp2D(left, vr) < 0)
                {
                    var polyLocalToWorld = navMeshQuery.PolygonLocalToWorldMatrix(polygonIds[apexIndex]);
                    var termPos = polyLocalToWorld.MultiplyPoint(apex + left);

                    n = RetracePortals(navMeshQuery, apexIndex, leftIndex, polygonIds, n, termPos, ref straightPath,
                        maxStraightPath);

                    //Debug.Log("LEFT");

                    if (n == maxStraightPath)
                    {
                        return straightPath[0].position;
                    }

                    apex = polyWorldToLocal.MultiplyPoint(termPos);
                    left.Set(0, 0, 0);
                    right.Set(0, 0, 0);
                    i = apexIndex = leftIndex;
                    continue;
                }

                if (Perp2D(right, vl) > 0)
                {
                    var polyLocalToWorld = navMeshQuery.PolygonLocalToWorldMatrix(polygonIds[apexIndex]);
                    var termPos = polyLocalToWorld.MultiplyPoint(apex + right);

                    n = RetracePortals(navMeshQuery, apexIndex, rightIndex, polygonIds, n, termPos, ref straightPath,
                        maxStraightPath);

                    //Debug.Log("RIGHT");

                    if (n == maxStraightPath)
                    {
                        return straightPath[0].position;
                    }

                    apex = polyWorldToLocal.MultiplyPoint(termPos);
                    left.Set(0, 0, 0);
                    right.Set(0, 0, 0);
                    i = apexIndex = rightIndex;
                    continue;
                }

                // Narrow funnel
                if (Perp2D(left, vl) >= 0)
                {
                    left = vl;
                    leftIndex = i;
                }

                if (Perp2D(right, vr) <= 0)
                {
                    right = vr;
                    rightIndex = i;
                }
            }

            // Remove the the next to last if duplicate point - e.g. start and end positions are the same
            // (in which case we have get a single point)
            if (n > 0 && (straightPath[n - 1].position.Equals(destination)))
                n--;

            n = RetracePortals(navMeshQuery, apexIndex, polySize - 1, polygonIds, n, destination, ref straightPath,
                maxStraightPath);

            if (n == maxStraightPath)
            {
                return straightPath[0].position;
            }

            return straightPath[0].position;

            // int counter = 0;
            // Vector3 left;
            // Vector3 right;
            // navMeshQuery.GetPortalPoints(polygonIds[1], polygonIds[2], out left, out right);
            // float3 position = (left + right) / 2;
            // Debug.Log(left + " - " + right);
            // return position;
            // RaycastHit collisionHit;

            // if (!Physics.Raycast(startPosition, startPosition - destination, out collisionHit,
            //     math.distance(startPosition, destination), 0))
            // {
            //     Debug.Log(startPosition + " - " + collisionHit.point);
            // }
            // else
            // {
            //     Debug.Log(startPosition + " NO COLLISION");
            // }

            // return float3.zero;
        }

        public static int RetracePortals(NavMeshQuery query, int startIndex, int endIndex, NativeSlice<PolygonId> path,
            int n, Vector3 termPos, ref NativeArray<NavMeshLocation> straightPath, int maxStraightPath)
        {
            for (var k = startIndex; k < endIndex - 1; ++k)
            {
                var type1 = query.GetPolygonType(path[k]);
                var type2 = query.GetPolygonType(path[k + 1]);
                if (type1 != type2)
                {
                    Vector3 l, r;
                    var status = query.GetPortalPoints(path[k], path[k + 1], out l, out r);
                    float3 cpa1, cpa2;
                    GeometryUtils.SegmentSegmentCPA(out cpa1, out cpa2, l, r, straightPath[n - 1].position, termPos);
                    straightPath[n] = query.CreateLocation(cpa1, path[k + 1]);

                    if (++n == maxStraightPath)
                    {
                        return maxStraightPath;
                    }
                }
            }

            straightPath[n] = query.CreateLocation(termPos, path[endIndex]);
            return ++n;
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
            var magnitude = math.sqrt(math.pow(vector.x, 2) + math.pow(vector.z, 2));
            return magnitude;
        }

        public static float Perp2D(Vector3 u, Vector3 v)
        {
            return u.z * v.x - u.x * v.z;
        }

        public static void Swap(ref Vector3 a, ref Vector3 b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var query = _query;

            float3 playerPosition=float3.zero;

            Entities
                .ForEach(
                    (ref Translation translation, ref PlayerTag tag) => { playerPosition = translation.Value; }).Run();

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                    {
                        if (math.distance(translation.Value, playerPosition) >= aiData.chaseDistance)
                        {
                            if (!aiData.changePosition)
                            {
                                var direction = new float3(aiData.position.x - aiData.finalPosition.x, 0,
                                    aiData.position.z - aiData.finalPosition.z);
                                if (Magnitude(direction) > 1)
                                {
                                    var path = FindPath(translation.Value, aiData.finalPosition, query,
                                        aiData.positionOffset);
                                    aiData.position = path;
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
                        }
                        else
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

    public class GeometryUtils
    {
        // Calculate the closest point of approach for line-segment vs line-segment.
        public static bool SegmentSegmentCPA(out float3 c0, out float3 c1, float3 p0, float3 p1, float3 q0,
            float3 q1)
        {
            var u = p1 - p0;
            var v = q1 - q0;
            var w0 = p0 - q0;

            float a = math.dot(u, u);
            float b = math.dot(u, v);
            float c = math.dot(v, v);
            float d = math.dot(u, w0);
            float e = math.dot(v, w0);

            float den = (a * c - b * b);
            float sc, tc;

            if (den == 0)
            {
                sc = 0;
                tc = d / b;

                // todo: handle b = 0 (=> a and/or c is 0)
            }
            else
            {
                sc = (b * e - c * d) / (a * c - b * b);
                tc = (a * e - b * d) / (a * c - b * b);
            }

            c0 = math.lerp(p0, p1, sc);
            c1 = math.lerp(q0, q1, tc);

            return den != 0;
        }
    }
}