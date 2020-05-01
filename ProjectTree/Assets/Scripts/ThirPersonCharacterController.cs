using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirPersonCharacterController : MonoBehaviour
{
    public float Speed;
    public CharacterController characterController;
    
    //Movimento BASE
    private float hor ;
    private float ver ;
    private Vector3 playerinput;
    private Vector3 movPlayer;
    
    //Gravedad
    public float gravity = 9.8f;
    public float VelCaida;
    
    //Salto
    public float jumpForce=5;

    //Movimiento por posicion de camara
    public Camera cam;
    private Vector3 camForward;
    private Vector3 camRight;
    
    //Disparo
    public GameObject Bullet;
    public GameObject LocFire;
    
    //Life
    public int life;


    private void Awake()
    {
        GameController.GetInstance().Player = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        
        
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        
        playerinput=new Vector3(hor,0,ver);
        playerinput= Vector3.ClampMagnitude(playerinput,1);
        
        CamDir();

        movPlayer = playerinput.x * camRight + playerinput.z * camForward;

        movPlayer = movPlayer * Speed;
        
        setGravity();
        Jump();       
        
    }

    private void FixedUpdate()
    {
        characterController.Move(movPlayer * Time.deltaTime);
    }


    void CamDir()
    {
        camForward = cam.transform.forward;
        camRight = cam.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }


    void setGravity()
    {
        if (characterController.isGrounded)
        {
            VelCaida = -gravity * Time.deltaTime;
            movPlayer.y = VelCaida;
        }
        else
        {
            VelCaida -= gravity * Time.deltaTime;
            movPlayer.y = VelCaida;
        }
    }

    void Jump()
    {
        if (characterController.isGrounded&& Input.GetButtonDown("Jump"))
        {
            VelCaida = jumpForce;
            movPlayer.y = VelCaida;
        }
    }

    void Shoot()
    {
        GameObject bulletShot;
        bulletShot= Instantiate(Bullet, LocFire.transform.position, Quaternion.identity);
        bulletShot.GetComponent<Rigidbody>().AddForce(transform.forward*20);
        bulletShot.GetComponent<Rigidbody>().velocity=transform.forward*20;
        bulletShot.transform.rotation = transform.rotation;
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        if(life<=0)
            GameController.GetInstance().gameOver();
    }
}
