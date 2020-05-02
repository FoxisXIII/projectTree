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
    public GameObject casco;
    
    public float time, speed;
    private bool now=false;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke("move",time);
    }

    void move()
    {
        Debug.Log("10Seg para auto destruccion" );
        now = true;
        //casco.GetComponent<BoxCollider>().isTrigger = true;
        //casco.SetActive(false);
        Destroy(casco,5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (casco != null)
        {
            if (now && casco.transform.position.y!=20f)
            {
                Debug.Log("Arriba plataforma");
                Vector3 newp = Vector3.MoveTowards(casco.transform.position,
                    new Vector3(casco.transform.position.x, 20, casco.transform.position.z), speed * Time.deltaTime);
                casco.transform.position = newp;
            }
        }
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
