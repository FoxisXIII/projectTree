using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validador : MonoBehaviour
{
    public enum Solucion
    {
        RED=0,BLUE=1,GREEN=2
    }

    public Solucion B1, B2, B3;
    public Material Mat;
    public Solucion bt1, bt2, bt3;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void Correccion()
    {
        if (B1==bt1)
        {
            if (B2==bt2)
            {
                if (B3==bt3)
                {
                    gameObject.GetComponent<MeshRenderer>().material = Mat;
                    Debug.Log("Correcto");
                }
            }
            
        }
    }
}
