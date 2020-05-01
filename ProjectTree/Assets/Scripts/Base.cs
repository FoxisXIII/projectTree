using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int life;

    //The base generates them by time?
    private int energyCreation;
    private int materialCreation;

    private void Awake()
    {
        GameController.GetInstance().Base = this;
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        if(life<=0)
            GameController.GetInstance().gameOver();
    }
}
