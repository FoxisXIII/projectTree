using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Hybrid;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public Animator hud;
    public GameObject ControlsPanel;

    public string enterMenuSoundPath;
    public string exitMenuSoundPath;

    private float userVolume;
    public Slider volumeSlider;
    private bool canPause;
    private float userFeelings;
    public Slider feelingsSlider;

    // Start is called before the first frame update
    void Start()
    {
        Resume();
        SoundManager.GetInstance().ChangeVolume(1);
        canPause = true;
        userVolume = PlayerPrefs.GetFloat("VOLUME", 1);
        volumeSlider.value = userVolume;
        ChangeVolume(userVolume);

        userFeelings = PlayerPrefs.GetFloat("FEELINGS", 1);
        feelingsSlider.value = userFeelings;
        ChangeFeelings(userFeelings);
    }

    // Update is called once per frame
    void Update()
    {
        if (canPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                    SoundManager.GetInstance().PlayOneShotSound(exitMenuSoundPath,
                        GameController.GetInstance().Player.transform.position);
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void Pause()
    {
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hud.SetBool("startPause", true);
        hud.SetBool("pause", true);
        GameController.GetInstance().pauseGame(true);
        canPause = false;
        SoundManager.GetInstance()
            .PlayOneShotSound(enterMenuSoundPath, GameController.GetInstance().Player.transform.position);
        SoundManager.GetInstance().ChangeVolume(0.2f);
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SoundManager.GetInstance().ChangeVolume(userVolume);
        GameController.GetInstance().gameOver("AT LEAST YOU TRIED...");
    }

    public void Resume()
    {
        if (!GameController.GetInstance().Player.cameraChanged)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        Time.timeScale = 1;
        GameIsPaused = false;
        GameController.GetInstance().pauseGame(false);
        hud.SetBool("pause", false);
        ChangeVolume(userVolume);
        ControlsPanel.SetActive(false);
        canPause = false;
    }

    public void ShowControls(bool show)
    {
        //hud.SetBool("pause", !show);
        ControlsPanel.SetActive(show);
    }

    public void ChangeVolume(float volume)
    {
        SoundManager.GetInstance().ChangeVolume(volume);
        GameController.GetInstance().userVolume = userVolume;
        userVolume = volume;
    }

    public void ChangeFeelings(float feelings)
    {
        GameController.GetInstance().Player.ChangeFeelings(feelings);
        GameController.GetInstance().userFeelings = userFeelings;
        userFeelings = feelings;
    }

    public void CanPause()
    {
        canPause = true;
    }
}