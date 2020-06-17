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
    [SerializeField] private GameObject enemyFlyBossPrefab;
    [SerializeField] private GameObject enemyGroundBossPrefab;
    private EntityManager _entityManager;
    private Entity _flyEnemyEntity;
    private Entity _groundEnemyEntity;
    private Entity _flyBossEnemyEntity;
    private Entity _groundBossEnemyEntity;
    private float time;
    private BlobAssetStore _blobAssetFly;
    private BlobAssetStore _blobAssetGround;
    private BlobAssetStore _blobAssetFlyBoss;
    private BlobAssetStore _blobAssetGroundBoss;
    public float3[] min, max;
    public ParticleSystem fog;
    private bool horde;
    private float timer;

    [Header("FMOD paths")] public string groundMovementSoundPath;
    public string airMovementSoundPath;
    public string attackPlayerSoundPath;
    public string attackBaseSoundPath;
    public string hitSoundPath;
    public string dieSoundPath;

    // Start is called before the first frame update
    void Start()
    {
        _blobAssetFly = new BlobAssetStore();
        _blobAssetGround = new BlobAssetStore();
        _blobAssetFlyBoss = new BlobAssetStore();
        _blobAssetGroundBoss = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _flyEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyFlyPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetFly));
        _groundEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyGroundPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetGround));
        _flyBossEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyFlyBossPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetFlyBoss));
        _groundBossEnemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyGroundBossPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetGroundBoss));
    }

    private void Update()
    {
        if (horde && GameController.GetInstance().CurrentEnemies == GameController.GetInstance().MaxWaveEnemies)
        {
            horde = false;
            var particle = fog.main;
            particle.startColor = new ParticleSystem.MinMaxGradient(Color.white);
        }
    }

    public void SpawnEnemy(bool horde)
    {
        if (horde && !this.horde)
        {
            this.horde = true;
            timer = 0;
            var particle = fog.main;
            particle.startColor = new ParticleSystem.MinMaxGradient(Color.red);
        }

        Entity enemy;

        if (Random.Range(0f, 1f) > .5f)
        {
            enemy = _entityManager.Instantiate(_flyEnemyEntity);
            SoundManager.GetInstance().PlayOneShotSound(airMovementSoundPath, enemy);
        }
        else
        {
            enemy = _entityManager.Instantiate(_groundEnemyEntity);
            SoundManager.GetInstance().PlayOneShotSound(groundMovementSoundPath, enemy);
        }


        var random = Random.Range(0f, 1f);

        var aiData = _entityManager.GetComponentData<AIData>(enemy);
        aiData.yOffset = aiData.canFly ? Random.Range(.25f, 2.5f) : 0;
        aiData.state = 0;
        aiData.attackDamage = horde ? 2 : 1;
        aiData.attackRate = Random.Range(.5f, 1f);
        aiData.horde = horde;
        aiData.canAttackPlayer = Random.Range(0f, 1f) < .25f;
        _entityManager.SetComponentData(enemy, aiData);

        var movementData = _entityManager.GetComponentData<MovementData>(enemy);
        var maxSpeed = 5 * (GameController.GetInstance().WaveCounter + 1);
        movementData.speed = Random.Range(200 + maxSpeed, math.min(250 + maxSpeed, 500));
        _entityManager.SetComponentData(enemy, movementData);


        var health = _entityManager.GetComponentData<HealthData>(enemy);
        health.value = 3+GameController.GetInstance().WaveCounter;
        health.maxValue = 3+GameController.GetInstance().WaveCounter;
        _entityManager.SetComponentData(enemy, health);

        _entityManager.SetComponentData(enemy,
            new Translation() {Value = GetPosition(min[0], max[0], random) + aiData.yOffset * Vector3.up});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});

        _entityManager.AddBuffer<EnemyPosition>(enemy).AddRange(GetAllPositions(random));
        _entityManager.AddBuffer<CollisionEnemy>(enemy);

        _entityManager.AddComponent(enemy, typeof(EnemyFMODPaths));
        _entityManager.SetComponentData(enemy, new EnemyFMODPaths
        {
            AttackBasePath = attackBaseSoundPath,
            AttackPlayerPath = attackPlayerSoundPath,
            HitPath = hitSoundPath,
            DiePath = dieSoundPath
        });

        GameController.GetInstance().AddEnemyWave();
    }

    public void SpawnBoss()
    {
        Entity enemy;

        if (Random.Range(0f, 1f) > .5f)
        {
            enemy = _entityManager.Instantiate(_flyBossEnemyEntity);
            SoundManager.GetInstance().PlayOneShotSound(airMovementSoundPath, enemy);
        }
        else
        {
            enemy = _entityManager.Instantiate(_groundBossEnemyEntity);
            SoundManager.GetInstance().PlayOneShotSound(groundMovementSoundPath, enemy);
        }


        var random = Random.Range(0f, 1f);

        var aiData = _entityManager.GetComponentData<AIData>(enemy);
        aiData.yOffset = aiData.canFly ? Random.Range(1f, 3f) : 0;
        aiData.state = 0;
        aiData.attackDamage = 20;
        aiData.attackRate = Random.Range(.5f, 1f);
        _entityManager.SetComponentData(enemy, aiData);
        
        var movementData = _entityManager.GetComponentData<MovementData>(enemy);
        movementData.speed = 100;
        _entityManager.SetComponentData(enemy, movementData);

        var health = _entityManager.GetComponentData<HealthData>(enemy);
        health.value = GameController.GetInstance().WaveCounter * 20;
        health.maxValue = GameController.GetInstance().WaveCounter * 20;
        _entityManager.SetComponentData(enemy, health);

        _entityManager.SetComponentData(enemy,
            new Translation() {Value = GetPosition(min[0], max[0], random) + aiData.yOffset * Vector3.up});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});

        _entityManager.AddBuffer<EnemyPosition>(enemy).AddRange(GetAllPositions(random));

        _entityManager.AddComponent(enemy, typeof(EnemyFMODPaths));
        _entityManager.SetComponentData(enemy, new EnemyFMODPaths
        {
            AttackBasePath = attackBaseSoundPath,
            AttackPlayerPath = attackPlayerSoundPath,
            HitPath = hitSoundPath,
            DiePath = dieSoundPath
        });

        GameController.GetInstance().AddEnemyWave();
    }

    private NativeArray<EnemyPosition> GetAllPositions(float random)
    {
        var list = new NativeList<EnemyPosition>(Allocator.Temp);
        for (int i = 1; i < min.Length; i++)
        {
            list.Add(new EnemyPosition() {position = GetPosition(min[i], max[i], random)});
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
        {
            _blobAssetFly.Dispose();
            _blobAssetGround.Dispose();
            _blobAssetFlyBoss.Dispose();
            _blobAssetGroundBoss.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        _blobAssetFly.Dispose();
        _blobAssetGround.Dispose();
        _blobAssetFlyBoss.Dispose();
        _blobAssetGroundBoss.Dispose();
    }
}