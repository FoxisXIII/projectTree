using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    private static GameController _instance;

    private int currentEnemies, maxWaveEnemies;
    
    private Base _base;
    private ThirPersonCharacterController _player;

    private GameController()
    {
        initiateWave();
    }

    public static GameController GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameController();
        }

        return _instance;
    }

    //TODO: Add the wavenumber/intensity of enemies/something?
    public void initiateWave()
    {
        currentEnemies = 0;
    }

    public void endWave()
    {
    }

    public void AddEnemyWave()
    {
        currentEnemies++;
        // Debug.Log(currentEnemies);
    }

    public void RemoveEnemyWave()
    {
        currentEnemies--;
    }

    public void pauseGame()
    {
    }

    public void gameOver()
    {
        Debug.Log("Game Over");
    }

    public int CurrentEnemies
    {
        get => currentEnemies;
        set => currentEnemies = value;
    }

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