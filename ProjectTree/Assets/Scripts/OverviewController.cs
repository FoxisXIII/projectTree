using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewController : MonoBehaviour
{
    public GameObject fpsCamera;
    public KeyCode cameraChange;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraChange))
        {
            GameController.GetInstance().Player.characterController.enabled = true;
            GameController.GetInstance().Player.fpsCamera.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
