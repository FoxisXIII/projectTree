using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    private static GameController instance;
    
    //private int waveNum;
    private GameController()
    {
        //waveNum=0;
    }

    public static GameController GetInstance()
    {
        if (instance == null)
        {
            instance = new GameController();
        }
        return instance;
    }

    //TODO: Add the wavenumber/intensity of enemies/something?
    public void initiateWave()
    {
        
    }

    public void endWave()
    {
        
    }

    public void pauseGame()
    {
        
    }
    
    public void gameOver()
    {
        
    }

}
