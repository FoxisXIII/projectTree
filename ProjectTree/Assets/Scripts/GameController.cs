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
    private bool _waveInProcess;

    private Base _base;
    private ThirdPersonCharacterController _player;
    private EntityCommandBuffer ecb;
    
    [Header("FMOD paths")] 
    public string startRoundSoundPath = "event:/FX/Round/Start";
    public string endRoundSoundPath = "event:/FX/Round/End";

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
            _maxWaveEnemies = Mathf.Min(1500, _maxWaveEnemies * 2);
        _enemiesSpawnRate /= 1.25f;
        _waveInProcess = true;
        SoundManager.GetInstance().PlayOneShotSound(startRoundSoundPath, _player.transform.position);
    }

    public void endWave()
    {
        _currentEnemies = 0;
        _diedEnemies = 0;
        UpdateResources(100);
        _player.recValue.text = RecursosA.ToString();
        _waveInProcess = false;
        if (_waveCounter != 0)
            SoundManager.GetInstance().PlayOneShotSound(endRoundSoundPath, _player.transform.position);
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
    
    public void retry()
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
        SceneManager.LoadScene("Scenario");
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