using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public string scene;
    public Text die;
    public Text killed, rounds, towers;
    public Text killedBest, roundsBest, towersBest;


    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        
        Debug.Log(PlayerPrefs.GetString("DIE"));
        die.text = PlayerPrefs.GetString("DIE");

        killedBest.text = "(" + PlayerPrefs.GetInt("KILLED") + ")";
        roundsBest.text = "(" + PlayerPrefs.GetInt("ROUNDS") + ")";
        towersBest.text = "(" + PlayerPrefs.GetInt("TURRETS") + ")";

        killed.text = GameController.GetInstance().EnemiesKilled.ToString();
        rounds.text = GameController.GetInstance().WaveCounter.ToString();
        towers.text = GameController.GetInstance().TowersPlaced.ToString();
    }

    public void changeScene()
    {
        GameController.GetInstance().DestroyEntities();
        SceneManager.LoadSceneAsync(scene);
    }
}