using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    public EnemySpawner[] northSpawners;
    public EnemySpawner[] southSpawners;
    public EnemySpawner[] eastSpawners;
    public EnemySpawner[] westSpawners;

    private List<EnemySpawner[]> _spawners;
    private bool[] _spawnersActivated;
    private bool _canSpawn, _canEndWave;
    private float _time;
    public float waveCooldown;
    public float enemySpawnRate;

    void Start()
    {
        _spawners = new List<EnemySpawner[]> {northSpawners, southSpawners, eastSpawners, westSpawners};
        _spawnersActivated = new[] {false, false, false, false};
        _spawnersActivated[Random.Range(0, _spawnersActivated.Length)] = true;
        GameController.GetInstance().MaxWaveEnemies = 50;
        GameController.GetInstance().EnemiesSpawnRate = enemySpawnRate;
        _canSpawn = false;
        _canEndWave = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartWave();

        SpawnEnemy();

        EndWave();

        _time += Time.deltaTime;
    }

    private void StartWave()
    {
        if (!_canSpawn && _time >= waveCooldown)
        {
            GameController.GetInstance().startWave();
            _canSpawn = true;
            _time = 0;
        }
    }

    private void EndWave()
    {
        if (_canEndWave && GameController.GetInstance().CurrentEnemies <= 0)
        {
            GameController.GetInstance().endWave();
            _canEndWave = false;
            _canSpawn = false;
        }
    }

    public void SpawnEnemy()
    {
        if (_canSpawn && !_canEndWave && _time >= GameController.GetInstance().EnemiesSpawnRate &&
            GameController.GetInstance().CurrentEnemies <
            GameController.GetInstance().MaxWaveEnemies)
        {
            var pos = Random.Range(0, _spawnersActivated.Length);
            while (!_spawnersActivated[pos])
            {
                pos = Random.Range(0, _spawnersActivated.Length);
            }

            var spawner = _spawners[pos];


            spawner[Random.Range(0, spawner.Length)].SpawnEnemy();

            _time = 0;
        }

        _canEndWave = GameController.GetInstance().CurrentEnemies >= GameController.GetInstance().MaxWaveEnemies;
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }
}