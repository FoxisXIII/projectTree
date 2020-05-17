using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public float Speed;
    public CharacterController characterController;

    //Movimento BASE
    private float hor;
    private float ver;
    private Vector3 playerinput;
    private Vector3 movPlayer;
    private float WalkSpeed;
    private float RunSpeed;
    public KeyCode RunKey = KeyCode.LeftShift;

    //Gravedad
    public float gravity = 9.8f;
    private float VelCaida;

    //Salto
    public float jumpForce = 50;

    //Movimiento por posicion de camara
    public Camera cam;
    private Vector3 camForward;
    private Vector3 camRight;

    //Disparo
    public GameObject Bullet;
    public GameObject LocFire;
    [Range(0, 1)] public float fireRate;
    private float timer;

    //ECS
    public bool useECS = false;
    private EntityManager manager;
    private Entity bulletEntityPrefab;
    private BlobAssetStore blobBullet;

    //Life
    public int life;
    public Text lifeText;
    
    //
    public int recursosA=200;
    public Text recValue;
    

    //Turret Spawner
    public Transform instantiateTurrets;
    public GameObject previewTurret;
    public GameObject shootingTurret;
    private Entity turretECS;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;
    private BlobAssetStore blobTurret;


    private void Awake()
    {
        GameController.GetInstance().Player = this;
        lifeText.text = life.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        WalkSpeed = Speed;
        RunSpeed = WalkSpeed * 2;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobTurret = new BlobAssetStore();
        turretECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(shootingTurret,
            GameObjectConversionSettings.FromWorld(manager.World, blobTurret));
        if (useECS)
        {
            blobBullet = new BlobAssetStore();
            bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Bullet,
                GameObjectConversionSettings.FromWorld(manager.World, blobBullet));
        }

        recValue.text = recursosA.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButton(0) && timer >= fireRate)
        {
            if (useECS)
            {
                ShootECS();
            }
            else
            {
                Shoot();
            }

            timer = 0f;
        }

        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                CreatePreviewTurret();
            }

            UpdatePreviewTurret();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CreateTurret();
        }
        
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CreatePreviewTurret();
            }

            UpdatePreviewTurret();
        }else if (Input.GetKeyUp(KeyCode.T))
        {
            CreateTurret();
        }

        hor = Input.GetAxis("Horizontal");

        ver = Input.GetAxis("Vertical");

        playerinput = new Vector3(hor, 0, ver);
        playerinput = Vector3.ClampMagnitude(playerinput, 1);

        CamDir();

        movPlayer = playerinput.x * camRight + playerinput.z * camForward;

        if (Input.GetKey(RunKey))
        {
            Speed = RunSpeed;
        }

        if (Input.GetKeyUp(RunKey))
        {
            Speed = WalkSpeed;
        }

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
        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            VelCaida = jumpForce;
            movPlayer.y = VelCaida;
        }
    }

    void Shoot()
    {
        GameObject bulletShot;
        bulletShot = Instantiate(Bullet, LocFire.transform.position, Quaternion.identity);
        bulletShot.GetComponent<Rigidbody>().AddForce(transform.forward * 20);
        bulletShot.GetComponent<Rigidbody>().velocity = transform.forward * 20;
        bulletShot.transform.rotation = transform.rotation;
    }

    void ShootECS()
    {
        Entity bullet = manager.Instantiate(bulletEntityPrefab);

        manager.SetComponentData(bullet, new Translation {Value = LocFire.transform.position});
        manager.SetComponentData(bullet, new Rotation {Value = LocFire.transform.rotation});
        manager.AddComponent(bullet, typeof(MovesForwardComponent));
    }


    public void ReceiveDamage(int damage)
    {
        life -= damage;
        lifeText.text = life.ToString();
        if (life <= 0)
            GameController.GetInstance().gameOver();
    }

    private void CreatePreviewTurret()
    {
        _instantiatedPreviewTurret = Instantiate(previewTurret, instantiateTurrets).GetComponent<PreviewTurret>();
    }

    private void UpdatePreviewTurret()
    {
        _turretCanBePlaced = _instantiatedPreviewTurret.isValidPosition();
        _instantiatedPreviewTurret.material.color = _turretCanBePlaced
            ? _instantiatedPreviewTurret.canBePlaced
            : _instantiatedPreviewTurret.canNotBePlaced;
    }

    private void CreateTurret()
    {
        Destroy(_instantiatedPreviewTurret.gameObject);

        if (_turretCanBePlaced&& recursosA>=20)
        {
            Entity turret = manager.Instantiate(turretECS);
            var position = instantiateTurrets.position;
            position.y += .5f;
            manager.SetComponentData(turret, new Translation {Value = position});
            manager.AddBuffer<EnemiesInRange>(turret);
            recursosA -= 20;
            recValue.text = recursosA.ToString();
        }
    }

    private void CreateTramp()
    {
        
    }

    private void OnDestroy()
    {
        blobBullet.Dispose();
        blobTurret.Dispose();
    }
}