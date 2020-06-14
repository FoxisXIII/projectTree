using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public string scene;
    public TextMeshProUGUI die;
    public TextMeshProUGUI killed, rounds, towers;
    public TextMeshProUGUI killedBest, roundsBest, towersBest;


    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
        die.SetText(PlayerPrefs.GetString("DIE"));

        killedBest.SetText(PlayerPrefs.GetInt("KILLED").ToString());
        roundsBest.SetText(PlayerPrefs.GetInt("ROUNDS").ToString());
        towersBest.SetText(PlayerPrefs.GetInt("TURRETS").ToString());

        killed.SetText(GameController.GetInstance().EnemiesKilled.ToString());
        rounds.SetText(GameController.GetInstance().WaveCounter.ToString());
        towers.SetText(GameController.GetInstance().TowersPlaced.ToString());
        
        GameController.GetInstance().EnemiesKilled = 0;
        GameController.GetInstance().TowersPlaced = 0;
        GameController.GetInstance().WaveCounter = 0;
        
    }

    public void changeScene()
    {
        GameController.GetInstance().DestroyEntities();
        SceneManager.LoadSceneAsync(scene);
    }
}