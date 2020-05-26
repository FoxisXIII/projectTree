using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    public Texture2D image;
    public int size;
    public Camera cam;
    public float maxAngle;
    public float minAngle;

    private float lookHeight;

    public void LookHeight(float value)
    {
        lookHeight = value;
        if (lookHeight>maxAngle)
        {
            lookHeight -= value;
        }
    }

    private void OnGUI()
    {
        
        Vector3 screenPositon = cam.WorldToScreenPoint(transform.position);
        screenPositon.y = Screen.height - screenPositon.y;
        GUI.DrawTexture(new Rect(screenPositon.x,screenPositon.y-lookHeight,size,size),image );
    }
}
