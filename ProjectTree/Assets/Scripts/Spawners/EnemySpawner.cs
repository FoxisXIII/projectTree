using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private EntityManager _entityManager;
    private Entity _enemyEntityPrefab;


    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore()));
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F))
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Entity enemy = _entityManager.Instantiate(_enemyEntityPrefab);
        _entityManager.SetComponentData(enemy, new Translation() {Value = transform.position});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});
    }
}