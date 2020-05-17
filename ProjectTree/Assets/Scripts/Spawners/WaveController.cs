using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    public EnemySpawner[] spawners;

    private bool[] _spawnersActivated;

    public bool[] SpawnersActivated => _spawnersActivated;

    private bool _canSpawn, _canEndWave;
    private float _time;
    public float waveCooldown;
    public float enemySpawnRate;

    [SerializeField] private Text nextRoundTimeText;
    [SerializeField] private Text roundText;
    [SerializeField] private Text currentEnemiesText;

    void Awake()
    {
        _spawnersActivated = new[] {false, false, false};
        _spawnersActivated[Random.Range(0, _spawnersActivated.Length)] = true;
        GameController.GetInstance().MaxWaveEnemies = 10;
        GameController.GetInstance().EnemiesSpawnRate = enemySpawnRate;
        _canEndWave = true;
    }

    private void Start()
    {
        EndWave();
    }

    // Update is called once per frame
    void Update()
    {
        StartWave();

        SpawnEnemy();

        EndWave();

        _time += Time.deltaTime;

        nextRoundTimeText.text = Math.Round((Decimal) (waveCooldown - _time), 2) + " s";
    }

    private void StartWave()
    {
        if (!_canSpawn && _time >= waveCooldown)
        {
            GameController.GetInstance().startWave();
            currentEnemiesText.transform.parent.parent.gameObject.SetActive(true);
            nextRoundTimeText.transform.parent.gameObject.SetActive(false);
            roundText.text = GameController.GetInstance().WaveCounter.ToString();

            for (int i = 0; i < _spawnersActivated.Length; i++) _spawnersActivated[i] = false;

            for (int i = 0;
                i <= Mathf.Min(GameController.GetInstance().WaveCounter / 5, _spawnersActivated.Length);
                i++)
            {
                var pos = Random.Range(0, _spawnersActivated.Length);
                while (!_spawnersActivated[pos])
                {
                    _spawnersActivated[pos] = true;
                    pos = Random.Range(0, _spawnersActivated.Length);
                }
            }

            _canSpawn = true;
            _time = 0;
        }
    }

    private void EndWave()
    {
        if (_canEndWave)
        {
            GameController.GetInstance().endWave();
            currentEnemiesText.transform.parent.parent.gameObject.SetActive(false);
            nextRoundTimeText.transform.parent.gameObject.SetActive(true);
            _canEndWave = false;
            _canSpawn = false;
            _time = 0;
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

            spawners[pos].SpawnEnemy();

            _time = 0;
        }

        currentEnemiesText.text =
            (GameController.GetInstance().MaxWaveEnemies - GameController.GetInstance().DiedEnemies).ToString();

        _canEndWave = GameController.GetInstance().DiedEnemies >= GameController.GetInstance().MaxWaveEnemies;
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }
}