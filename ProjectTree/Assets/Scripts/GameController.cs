using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController
{
    private static GameController _instance;

    private int _currentEnemies, _diedEnemies, _maxWaveEnemies, _waveCounter;
    private float _enemiesSpawnRate;

    private Base _base;
    private ThirdPersonCharacterController _player;
    private EntityCommandBuffer ecb;

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
        _waveCounter++;
        if (_waveCounter > 1)
            _maxWaveEnemies = Mathf.Min(1500, _maxWaveEnemies * 2);
        _enemiesSpawnRate /= 1.25f;
    }

    public void endWave()
    {
        _currentEnemies = 0;
        _diedEnemies = 0;
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
        _waveCounter = 0;
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ClearEntities>().Update();
        if (World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            var systems = World.DefaultGameObjectInjectionWorld.Systems;
            foreach (var s in systems)
            {
                s.Enabled = false;
            }
            World.DefaultGameObjectInjectionWorld.Dispose();
        }
 
        DefaultWorldInitialization.Initialize("Default World", false);
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