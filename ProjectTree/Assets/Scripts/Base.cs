using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float life;

    private float maxLife;

    //The base generates them by time?
    private int energyCreation;
    private int materialCreation;

    public Image lifeUI_1, lifeUI_2;


    private void Awake()
    {
        GameController.GetInstance().Base = this;
        maxLife = life;
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        lifeUI_1.fillAmount = life / maxLife;

        if (life <= 0)
            GameController.GetInstance().gameOver("THE DRONS ENTERED!");
    }
}