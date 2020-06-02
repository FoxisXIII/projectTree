using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.WSA;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyFlyPrefab;
    [SerializeField] private GameObject enemyGroundPrefab;
    private EntityManager _entityManager;
    private Entity _flyEnemyEntity;
    private Entity _groundEnemyEntity;
    private float time;
    private BlobAssetStore _blobAssetFly;
    private BlobAssetStore _blobAssetGround;
    public float3[] min, max;

    // Start is called before the first frame update
    void Start()
    {
        _blobAssetFly = new BlobAssetStore();
        _blobAssetGround = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _flyEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyFlyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetFly));
        _groundEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyGroundPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetFly));
    }

    public void SpawnEnemy()
    {
        Entity enemy;

        if (Random.Range(0f, 1f) > .5f)
            enemy = _entityManager.Instantiate(_flyEnemyEntity);
        else
            enemy = _entityManager.Instantiate(_groundEnemyEntity);


        var random = Random.Range(0f, 1f);

        var aiData = _entityManager.GetComponentData<AIData>(enemy);
        if (aiData.canFly)

            aiData.state = 0;
        Vector3 offset = aiData.canFly ? Vector3.up * Random.Range(1f, 5f) : Vector3.zero;
        _entityManager.SetComponentData(enemy, aiData);

        _entityManager.SetComponentData(enemy,
            new Translation() {Value = GetPosition(min[0], max[0], random) + offset});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});

        _entityManager.AddBuffer<EnemyPosition>(enemy).AddRange(GetAllPositions(random, offset));
        _entityManager.AddBuffer<CollisionEnemy>(enemy);

        GameController.GetInstance().AddEnemyWave();
    }

    private NativeArray<EnemyPosition> GetAllPositions(float random, Vector3 offset)
    {
        var list = new NativeList<EnemyPosition>(Allocator.Temp);
        for (int i = 1; i < min.Length; i++)
        {
            list.Add(new EnemyPosition() {position = GetPosition(min[i], max[i], random) + offset});
        }
        return list;
    }

    private Vector3 GetPosition(float3 min, float3 max, float randomValue)
    {
        float distance = 0;
        distance = max.x - min.x;
        min.x += distance * randomValue;
        distance = max.y - min.y;
        min.y += distance * randomValue;
        distance = max.z - min.z;
        min.z += distance * randomValue;
        
        // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.position = min;

        return min;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < min.Length; i++)
        {
            if (i < min.Length - 1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(min[i], min[i + 1]);
                Gizmos.DrawLine(max[i], max[i + 1]);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(min[i], 1);
            Gizmos.DrawSphere(max[i], 1);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(min[i], max[i]);
        }
    }

    public void Dispose()
    {
        if (GameController.GetInstance().Player.life <= 0)
            _blobAssetFly.Dispose();
    }

    private void OnApplicationQuit()
    {
        _blobAssetFly.Dispose();
    }
}