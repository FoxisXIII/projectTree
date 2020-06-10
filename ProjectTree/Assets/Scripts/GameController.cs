﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController
{
    private static GameController _instance;

    private int _currentEnemies, _diedEnemies, _maxWaveEnemies, _waveCounter, _enemiesKilled, _towersPlaced;
    private float _enemiesSpawnRate;
    private bool _waveInProcess, _normalWave, _bossWave;

    private Base _base;
    private ThirdPersonCharacterController _player;
    private EntityCommandBuffer ecb;

    [Header("FMOD paths")] public string startRoundSoundPath = "event:/FX/Round/Start";
    public string endRoundSoundPath = "event:/FX/Round/End";

    //Recursos
    private int _iron = 200;
    private Dictionary<string, List<Material>> animationMaterials;
    private int _beforeBossMaxWaveEnemies;
    private int _numberOfBoses;
    private bool _noBaseDamage;

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

    public int iron
    {
        get => _iron;
    }

    public void UpdateResources(int value)
    {
        _iron += value;
        _player.ironText.SetText("Iron " + iron);
    }

    public void startWave()
    {
        _waveCounter++;


        if (_waveCounter >= 1)
        {
            if (_waveCounter % 5 == 0)
            {
                _beforeBossMaxWaveEnemies = _maxWaveEnemies;
                _numberOfBoses = Mathf.Min(3, 1 + (1 * (_waveCounter / 15)));
                _maxWaveEnemies = _numberOfBoses + Mathf.Min(250, 25 * ((_waveCounter / 5) - 1));
                _bossWave = true;
            }
            else if (_waveCounter > 1)
            {
                _maxWaveEnemies = Mathf.Min(250, _maxWaveEnemies + 10);
                _normalWave = true;
            }
            else
                _normalWave = true;
        }

        _noBaseDamage = true;
        _enemiesSpawnRate = Mathf.Max(.5f, _enemiesSpawnRate / 1.1f);
        _waveInProcess = true;
        SoundManager.GetInstance().PlayOneShotSound(startRoundSoundPath, _player.transform.position);
    }

    public void endWave()
    {
        _currentEnemies = 0;
        _diedEnemies = 0;
        UpdateResources(100);
        if (_noBaseDamage)
            _base.Heal(100);
        if (_bossWave)
            _maxWaveEnemies = _beforeBossMaxWaveEnemies;

        _waveInProcess = false;
        _normalWave = false;
        _bossWave = false;
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
        _enemiesKilled++;
    }

    public void pauseGame()
    {
    }

    public void gameOver(string text)
    {
        DestroyEntities();
        if (PlayerPrefs.GetInt("KILLED") < _enemiesKilled)
        {
            PlayerPrefs.SetInt("KILLED", _enemiesKilled);
        }

        if (PlayerPrefs.GetInt("ROUNDS") < _waveCounter)
        {
            PlayerPrefs.SetInt("ROUNDS", _waveCounter);
        }

        if (PlayerPrefs.GetInt("TOWERS") < _towersPlaced)
        {
            PlayerPrefs.SetInt("TOWERS", _towersPlaced);
        }

        PlayerPrefs.SetString("DIE", text);

        SceneManager.LoadScene("Game Over");
    }

    public void DestroyEntities()
    {
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
    }

    #region Getters and Setters

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

    public int WaveCounter
    {
        get => _waveCounter;
        set => _waveCounter = value;
    }

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

    public int EnemiesKilled
    {
        get => _enemiesKilled;
        set => _enemiesKilled = value;
    }

    public int TowersPlaced
    {
        get => _towersPlaced;
        set => _towersPlaced = value;
    }

    public bool NoBaseDamage
    {
        get => _noBaseDamage;
        set => _noBaseDamage = value;
    }

    public bool NormalWave => _normalWave;

    public bool BossWave => _bossWave;

    public bool WaveInProcess => _waveInProcess;

    public int NumberOfBoses => _numberOfBoses;

    #endregion
}