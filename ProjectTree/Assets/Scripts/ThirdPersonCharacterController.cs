using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
    [Range(0, 1)] public float initFireRate;
    [HideInInspector] public float fireRate;
    private float timer;

    //ECS
    public bool useECS = false;
    private EntityManager manager;
    private Entity bulletEntityPrefab;
    private BlobAssetStore blobBullet;

    //Life
    public int maxLife;
    [HideInInspector] public int life;
    public Text lifeText;

    //
    public int recursosA = 200;
    public Text recValue;


    //Turret Spawner
    public Transform instantiateTurrets;
    public GameObject previewTurret;
    public GameObject shootingTurret;
    private Entity turretECS;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;
    private BlobAssetStore blobTurret;
    
    //Trap spawner
    public GameObject previewTrap;
    public GameObject trap;
    private PreviewTurret _instantiatedPreviewTrap;
    private Entity trapECS;
    private BlobAssetStore blobTrap;

    //Buffs
    [HideInInspector] public bool hasBuff;
    public float initialDamage;
    private float damage;
    private bool shotgun;
    private int shotgunRange;


    private void Awake()
    {
        GameController.GetInstance().Player = this;
        life = maxLife / 2;
        lifeText.text = life.ToString();
        StopBuffs();
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobTurret = new BlobAssetStore();
        blobTrap = new BlobAssetStore();
        turretECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(shootingTurret,
            GameObjectConversionSettings.FromWorld(manager.World, blobTurret));
        trapECS=GameObjectConversionUtility.ConvertGameObjectHierarchy(trap,
            GameObjectConversionSettings.FromWorld(manager.World, blobTrap));
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
                if (shotgun)
                    ShotgunECS(LocFire.transform.position, LocFire.transform.rotation.eulerAngles);
                else
                    ShootECS(LocFire.transform.position, LocFire.transform.rotation);
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
                CreatePreviewTap();
            }

            UpdatePreviewTrap();
        }else if (Input.GetKeyUp(KeyCode.T))
        {
            CreateTramp();
        }

        hor = Input.GetAxis("Horizontal");

        ver = Input.GetAxis("Vertical");

        playerinput = new Vector3(hor, 0, ver);
        playerinput = Vector3.ClampMagnitude(playerinput, 1);

        CamDir();

        movPlayer = playerinput.x * camRight + playerinput.z * camForward;
        float speed = WalkSpeed;
        if (Input.GetKey(RunKey))
        {
            speed = RunSpeed;
        }

        if (Input.GetKeyUp(RunKey))
        {
            speed = WalkSpeed;
        }

        movPlayer = movPlayer * speed;

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

    void ShootECS(Vector3 position, Quaternion rotation)
    {
        Entity bullet = manager.Instantiate(bulletEntityPrefab);

        manager.SetComponentData(bullet, new Translation {Value = position});
        manager.SetComponentData(bullet, new Rotation {Value = rotation});
        var damage = manager.GetComponentData<DealsDamage>(bullet);
        damage.Value = this.damage;
        manager.SetComponentData(bullet, damage);
        manager.AddComponent(bullet, typeof(MovesForwardComponent));
    }

    void ShotgunECS(Vector3 position, Vector3 rotation)
    {
        int max = shotgunRange / 2;
        int min = -max;
        int totalAmount = shotgunRange * shotgunRange;

        Vector3 tempRot = rotation;
        int index = 0;

        NativeArray<Entity> bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);
        manager.Instantiate(bulletEntityPrefab, bullets);

        for (int x = min; x < max; x++)
        {
            tempRot.x = (rotation.x + 3 * x) % 360;
            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + 3 * y) % 360;
                manager.SetComponentData(bullets[index], new Translation {Value = position});
                manager.SetComponentData(bullets[index], new Rotation {Value = Quaternion.Euler(tempRot)});
                var damage = manager.GetComponentData<DealsDamage>(bullets[index]);
                damage.Value = this.damage;
                manager.SetComponentData(bullets[index], damage);
                manager.AddComponent(bullets[index], typeof(MovesForwardComponent));
                index++;
            }
        }

        bullets.Dispose();
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
    private void CreatePreviewTap()
    {
        _instantiatedPreviewTrap = Instantiate(previewTrap, instantiateTurrets).GetComponent<PreviewTurret>();
    }

    private void UpdatePreviewTurret()
    {
        _turretCanBePlaced = _instantiatedPreviewTurret.isValidPosition();
        _instantiatedPreviewTurret.material.color = _turretCanBePlaced
            ? _instantiatedPreviewTurret.canBePlaced
            : _instantiatedPreviewTurret.canNotBePlaced;
    }
    private void UpdatePreviewTrap()
    {
        _turretCanBePlaced = _instantiatedPreviewTrap.isValidPosition();
        _instantiatedPreviewTrap.material.color = _turretCanBePlaced
            ? _instantiatedPreviewTrap.canBePlaced
            : _instantiatedPreviewTrap.canNotBePlaced;
    }

    private void CreateTurret()
    {
        Destroy(_instantiatedPreviewTurret.gameObject);

        if (_turretCanBePlaced && recursosA >= 20)
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
        Destroy(_instantiatedPreviewTrap.gameObject);

        if (_turretCanBePlaced&& recursosA>=20)
        {
            Entity trap = manager.Instantiate(trapECS);
            var position = instantiateTurrets.position;
            position.y += 0f;
            manager.SetComponentData(trap, new Translation {Value = position});
            manager.SetComponentData(trap, new Rotation {Value = transform.rotation});
            manager.AddBuffer<EnemiesInRange>(trap);
            recursosA -= 20;
            recValue.text = recursosA.ToString();
        }
    }

    private void OnDestroy()
    {
        blobBullet.Dispose();
        blobTurret.Dispose();
    }

    public void RecoverHealth(int health)
    {
        StopBuffs();
        life = Mathf.Min(life + health, maxLife);
        lifeText.text = life.ToString();
    }

    public void IncreaseResources(int resources)
    {
        StopBuffs();
        recursosA += resources;
        recValue.text = recursosA.ToString();
    }

    public void IncreaseAttack(int Attack)
    {
        if (!hasBuff)
        {
            StopBuffs();
            damage = initialDamage * Attack;
        }
    }

    public void IncreaseSpeed(int Speed)
    {
        if (!hasBuff)
        {
            StopBuffs();
            WalkSpeed = this.Speed * Speed;
            RunSpeed = WalkSpeed * 2;
            fireRate = initFireRate / Speed;
        }
    }

    public void Shotgun(int shotgun)
    {
        if (!hasBuff)
        {
            StopBuffs();
            this.shotgun = true;
            shotgunRange = shotgun;
        }
    }

    public void StopBuffs()
    {
        WalkSpeed = this.Speed;
        RunSpeed = WalkSpeed * 2;
        fireRate = initFireRate;
        damage = initialDamage;
        shotgun = false;
    }
}