using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneNoButton : MonoBehaviour
{
    public string scene;
    public GameObject button;
    public GameObject loading;
    public float maxTime;
    private float time;
    private bool stop;

    public Image slider;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (!stop)
        {
            time += Time.deltaTime;
            if (time >= maxTime)
            {
                ActivateButton();
                stop = true;
            }

            slider.fillAmount = time / maxTime;
        }
    }

    public void ActivateButton()
    {
        //SceneManager.LoadSceneAsync(scene);
        button.SetActive(true);
        loading.SetActive(false);
    }
}