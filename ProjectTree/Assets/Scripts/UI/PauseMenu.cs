using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Hybrid;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenu,countime;
    
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
        for (int i = 0; i < transform.childCount; i++)
        {
            if(!countime.Equals(transform.GetChild(i).gameObject))
                transform.GetChild(i).gameObject.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;

    }

    public void Restart()
    {
        Time.timeScale = 1;
        GameController.GetInstance().retry();
    }

    public void Resume()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(!countime.Equals(transform.GetChild(i).gameObject))
                transform.GetChild(i).gameObject.SetActive(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
