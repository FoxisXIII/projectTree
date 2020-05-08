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
    [SerializeField] private GameObject enemyPrefab;
    private EntityManager _entityManager;
    private Entity _enemyEntityPrefab;
    private float time;
    private BlobAssetStore blobAssetStore;
    public float2 spawnMin;
    public float2 spawnMax;
    public float3 finalPositionLeft, finalPositionRight;

    // Start is called before the first frame update
    void Start()
    {
        blobAssetStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
        SpawnEnemy();
    }


    public void SpawnEnemy()
    {
        Entity enemy = _entityManager.Instantiate(_enemyEntityPrefab);


        _entityManager.SetComponentData(enemy,
            new Translation()
                {Value = GetRandomPosition(transform.position, spawnMin.x, spawnMax.x, spawnMin.y, spawnMax.y)});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});
        var aiData = _entityManager.GetComponentData<AIData>(enemy);
        aiData.state = 0;
        aiData.direction = (spawnMax.x == spawnMin.x) ? Vector3.forward : Vector3.right;
        aiData.finalPosition = GetRandomPosition(finalPositionLeft, finalPositionLeft.x, finalPositionRight.x,
            finalPositionLeft.z,
            finalPositionRight.z);
        aiData.canAttackPlayer = Random.Range(0f, 1f) > .75f;
        _entityManager.SetComponentData(enemy, aiData);

        GameController.GetInstance().AddEnemyWave();
    }

    private Vector3 GetRandomPosition(Vector3 position, float xMin, float xMax, float yMin, float yMax)
    {
        if (yMin == yMax)
            position.x = Random.Range(xMin, xMax);
        if (xMin == xMax)
            position.z = Random.Range(yMax, yMax);
        return position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(spawnMin.x, 0, spawnMin.y), new Vector3(spawnMax.x, 0, spawnMax.y));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(finalPositionLeft, finalPositionRight);
    }

    private void OnDestroy()
    {
        if (GameController.GetInstance().Player.life > 0)
            blobAssetStore.Dispose();
    }

    private void OnApplicationQuit()
    {
        blobAssetStore.Dispose();
    }
}