using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string scene;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void changeScene()
    {
        GameController.GetInstance().DestroyEntities();
        SceneManager.LoadSceneAsync(scene);
    }
}