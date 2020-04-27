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

    public void receiveDamage(int damage)
    {
        life -= damage;
        if(life<=0)
            GameController.GetInstance().gameOver();
    }
}
