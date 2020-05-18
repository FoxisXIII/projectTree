using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController
{
    private static GameController _instance;

    private int _currentEnemies, _diedEnemies, _maxWaveEnemies, _waveCounter;
    private float _enemiesSpawnRate;
    private bool _waveInProcess;

    private Base _base;
    private ThirdPersonCharacterController _player;
    
    //Recursos
    private int _recursosA = 200;

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

    public int RecursosA
    {
        get => _recursosA;
    }

    public void UpdateResources(int value)
    {
        _recursosA += value;
        _player.recValue.text = RecursosA.ToString();
    }

    public void startWave()
    {
        _waveCounter++;
        if (_waveCounter > 1)
            _maxWaveEnemies *= 2;
        _enemiesSpawnRate /= 1.25f;
        _waveInProcess = true;
    }

    public void endWave()
    {
        _currentEnemies = 0;
        _diedEnemies = 0;
        UpdateResources(100);
        _player.recValue.text = RecursosA.ToString();
        _waveInProcess = false;
    }

    public void AddEnemyWave()
    {
        _currentEnemies++;
    }

    public void RemoveEnemyWave()
    {
        _diedEnemies++;
    }

    public bool WaveInProcess => _waveInProcess;

    public void pauseGame()
    {
    }

    public void gameOver()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var allEntities = entityManager.GetAllEntities();
        foreach (var entity in allEntities)
        {
            entityManager.DestroyEntity(entity);
        }
        allEntities.Dispose();
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

    public ThirdPersonCharacterController Player
    {
        get => _player;
        set => _player = value;
    }
}