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
    public float3 finalPosition;

    // Start is called before the first frame update
    void Start()
    {
        blobAssetStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
    }


    public void SpawnEnemy()
    {
        Entity enemy = _entityManager.Instantiate(_enemyEntityPrefab);

        var position = transform.position;
        if (spawnMin.y == spawnMax.y)
            position.x = Random.Range(spawnMin.x, spawnMax.x);
        if (spawnMin.x == spawnMax.x)
            position.z = Random.Range(spawnMin.y, spawnMax.y);

        _entityManager.SetComponentData(enemy, new Translation() {Value = position});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});
        var aiData = _entityManager.GetComponentData<AIData>(enemy);
        aiData.state = 0;
        aiData.positionOffset = (transform.position - position) / 2;
        aiData.finalPosition = finalPosition;
        aiData.canAttackPlayer = Random.Range(0f, 1f) > .75f;
        _entityManager.SetComponentData(enemy, aiData);

        GameController.GetInstance().AddEnemyWave();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(spawnMin.x, 0, spawnMin.y));
        Gizmos.DrawLine(transform.position, new Vector3(spawnMax.x, 0, spawnMax.y));
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(finalPosition, 1f);
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