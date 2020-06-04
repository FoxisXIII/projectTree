using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Hybrid;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public Animator hud;
    
    public string enterMenuSoundPath;
    public string exitMenuSoundPath;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
                Pause();
        }
    }

    public void Pause()
    {
        SoundManager.GetInstance().PlayOneShotSound(enterMenuSoundPath, GameController.GetInstance().Player.transform);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hud.SetBool("startPause", true);
        hud.SetBool("pause", true);
        GameIsPaused = true;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        GameController.GetInstance().gameOver("YOU QUIT!");
    }

    public void Resume()
    {
        SoundManager.GetInstance().PlayOneShotSound(exitMenuSoundPath, GameController.GetInstance().Player.transform);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        hud.SetBool("pause", false);
        GameIsPaused = false;
    }
}