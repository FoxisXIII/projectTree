using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController
{
    private static GameController _instance;

    private int _currentEnemies,_diedEnemies, _maxWaveEnemies, _waveCounter;
    private float _enemiesSpawnRate;

    private Base _base;
    private ThirPersonCharacterController _player;

    private GameController()
    {
    }

    public static GameController GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameController();
        }

        return _instance;
    }

    public void startWave()
    {
        if (_waveCounter > 1)
            _maxWaveEnemies *= 2;
        _enemiesSpawnRate /= 1.25f;
        _waveCounter++;
    }

    public void endWave()
    {
        _currentEnemies = 0;
        _player.recursosA += 100;
        _player.recValue.text = _player.recursosA.ToString();
    }

    public void AddEnemyWave()
    {
        _currentEnemies++;
    }

    public void RemoveEnemyWave()
    {
        _diedEnemies++;
    }

    public void pauseGame()
    {
    }

    public void gameOver()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.DestroyEntity(entityManager.GetAllEntities());
        SceneManager.LoadScene("Game Over");
    }

    public int CurrentEnemies
    {
        get => _currentEnemies;
        set => _currentEnemies = value;
    }
    public int DiedEnemies
    {
        get => _diedEnemies;
        set => _diedEnemies = value;
    }

    public int MaxWaveEnemies
    {
        get => _maxWaveEnemies;
        set => _maxWaveEnemies = value;
    }

    public float EnemiesSpawnRate
    {
        get => _enemiesSpawnRate;
        set => _enemiesSpawnRate = value;
    }

    public int WaveCounter => _waveCounter;

    public Base Base
    {
        get => _base;
        set => _base = value;
    }

    public ThirPersonCharacterController Player
    {
        get => _player;
        set => _player = value;
    }
}