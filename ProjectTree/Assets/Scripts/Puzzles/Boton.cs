using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton : MonoBehaviour
{
    public Material []color= new Material[3];

    public Validador cor;
    public int colorA, BotonAm;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E presionado");
                if (colorA == 2) colorA = -1;
                colorA++;
                gameObject.GetComponent<MeshRenderer>().material = color[colorA];
                switch (BotonAm)
                {
                    case 0:
                        switch (colorA)
                        {
                        case 0:
                            cor.bt1 = Validador.Solucion.RED; break;
                        case 1:
                            cor.bt1 = Validador.Solucion.BLUE; break;
                        default:
                            cor.bt1 = Validador.Solucion.GREEN; break;
                        }
                        Debug.Log("Boton 1 cambiado");
                        break;
                    
                    case 1:
                        switch (colorA)
                        {
                            case 0:
                                cor.bt2 = Validador.Solucion.RED; break;
                            case 1:
                                cor.bt2 = Validador.Solucion.BLUE; break;
                            default:
                                cor.bt2 = Validador.Solucion.GREEN; break;
                        }
                        Debug.Log("Boton 2 cambiado");
                        break;
                    
                    default:
                        switch (colorA)
                        {
                            case 0:
                                cor.bt3 = Validador.Solucion.RED; break;
                            case 1:
                                cor.bt3 = Validador.Solucion.BLUE; break;
                            default:
                                cor.bt3 = Validador.Solucion.GREEN; break;
                        }
                        Debug.Log("Boton 3 cambiado");
                        break;
                }
                cor.Correccion();
            }
        }
    }
}
