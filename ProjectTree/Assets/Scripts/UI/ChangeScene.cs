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
        SoundManager.GetInstance().PlayOneShotSound("event:/FX/Menu/Select", transform.position);
        SceneManager.LoadSceneAsync("Scenes/"+scene);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.Exit(0);
#else
        Application.Quit();
#endif
    }
}