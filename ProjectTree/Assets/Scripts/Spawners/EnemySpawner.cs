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

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private EntityManager _entityManager;
    private Entity _enemyEntityPrefab;
    private float time;
    private BlobAssetStore blobAssetStore;
    public float3 finalPosition;

    // Start is called before the first frame update
    void Start()
    {
        blobAssetStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 1)
        // if (Input.GetKeyDown(KeyCode.F))
            SpawnEnemy();
        time += Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        time = 0;
        Entity enemy = _entityManager.Instantiate(_enemyEntityPrefab);

        var position = transform.position;
        _entityManager.SetComponentData(enemy, new Translation() {Value = position});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});
        _entityManager.SetComponentData(enemy, new AIData()
        {
            state = 0,
            finalPosition = finalPosition,
        });
    }

    private void OnApplicationQuit()
    {
        blobAssetStore.Dispose();
    }
}