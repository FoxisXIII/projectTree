using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float life;
    
    [SerializeField]
    private Text healthText;

    //The base generates them by time?
    private int energyCreation;
    private int materialCreation;
    
    public Image LifeImage;


    private void Awake()
    {
        GameController.GetInstance().Base = this;
        healthText.text = life.ToString();
    }

    private void Update()
    {
        if(life<=0)
            GameController.GetInstance().gameOver();
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        healthText.text = life.ToString();
        var color=LifeImage.color;
        Debug.Log(life/1000);
        color.a = life/1000;
        LifeImage.color = color;
        if(life<=0)
            GameController.GetInstance().gameOver();

    }
}
