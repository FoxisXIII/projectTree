using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int life;
    
    [SerializeField]
    private Text healthText;

    //The base generates them by time?
    private int energyCreation;
    private int materialCreation;

    private void Awake()
    {
        GameController.GetInstance().Base = this;
        healthText.text = life.ToString();
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        healthText.text = life.ToString();
        if(life<=0)
            GameController.GetInstance().gameOver();
    }
}
