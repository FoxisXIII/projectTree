using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
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
    [Header("Movimiento")]
    private float turnSmoothVelocity;
    public float turnSmoothTime=0.1f;
    private float hor;
    private float ver;
    private Vector3 movPlayer;
    private Vector3 moveDir;
    public Transform cam;
    private float WalkSpeed;
    private float RunSpeed;
    private float speedper;
    public KeyCode RunKey = KeyCode.LeftShift;

    //Gravedad
    public float gravity = 9.8f;
    private float VelCaida;

    //Salto
    public float jumpForce = 50;

    //Movimiento por posicion de camara

    //Disparo
    [Header("Shoot")]
    public GameObject Bullet;
    public GameObject LocFire;
    [Range(0, 1)] public float initFireRate;
    [HideInInspector] public float fireRate;
    private float timer;

    //ECS
    [Header("ECS")]
    public bool useECS = false;
    private EntityManager manager;
    private Entity bulletEntityPrefab;
    private BlobAssetStore blobBullet;

    //Life
    [Header("LIFE")]
    public float maxLife;
    [HideInInspector] public float life;
    public Image lifeImage;
    public TextMeshProUGUI ironText;


    //Turret Spawner
    [Header("Spawner")]
    public Transform instantiateTurrets;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;

    //Trap spawner
    public GameObject previewTrap;
    public GameObject trap;
    private PreviewTurret _instantiatedPreviewTrap;
    private Entity trapECS;
    private BlobAssetStore blobTrap;

    //Buffs
    [Header("BUFF")]
    [HideInInspector] public bool hasBuff;
    [HideInInspector] public Entity buffEntity;
    public int initialDamage;
    private int damage;
    private bool shotgun;
    private int shotgunRange;


    //Enemies Attacking
    private Dictionary<Entity, Vector3> enemies;


    //Change Camera
    [Header("Camara")]
    public GameObject fpsCamera;
    public GameObject birdCamera;
    public Animator hud;
    public KeyCode cameraChange;
    public bool cameraChanged;
    private Vector3 initialPosition;
    
    [HideInInspector] public string lastAnimatorKey;

    [Header("Intento de vertical")] 
    public Transform chest;
    public float speedRotation;
    public CinemachineFreeLook cine;
    

    private float minRotate = -18f, maxRotate = 1f;
    /*
    public Transform Target;
    public Vector3 Offset;
    private Animator anim;
    private Transform chest;*/

    
    private void Awake()
    {
        enemies = new Dictionary<Entity, Vector3>();
        GameController.GetInstance().Player = this;
        StopBuffs();

        initialPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
       /* anim = GetComponent<Animator>();
        chest = anim.GetBoneTransform(HumanBodyBones.Chest);*/
        
        
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobTrap = new BlobAssetStore();
        trapECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(trap,
            GameObjectConversionSettings.FromWorld(manager.World, blobTrap));
        if (useECS)
        {
            blobBullet = new BlobAssetStore();
            bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Bullet,
                GameObjectConversionSettings.FromWorld(manager.World, blobBullet));
        }
        
        

        GameController.GetInstance().UpdateResources(0);
    }

    // Update is called once per frame
    void Update()
    {
        lifeImage.fillAmount = (float) life / (float) maxLife;

        if (life <= 0)
            GameController.GetInstance().gameOver("KILLED BY X AE A12");

        if (Input.GetKeyDown(cameraChange) && !cameraChanged && !GameController.GetInstance().WaveInProcess)
        {
            birdCamera.transform.position = fpsCamera.transform.position;
            birdCamera.transform.rotation = fpsCamera.transform.rotation;
            birdCamera.SetActive(true);
            characterController.enabled = false;
            fpsCamera.SetActive(false);
            hud.SetBool("towers", true);

            if (hud.GetBool("inRound"))
            {
                lastAnimatorKey = "inRound";
                hud.SetBool("inRound", false);
            }
            else if (hud.GetBool("nextRound"))
            {
                lastAnimatorKey = "nextRound";
                hud.SetBool("nextRound", false);
            }

            cameraChanged = true;
        }

        if (!cameraChanged)
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
                    CreatePreviewTrap();
                }

                UpdatePreviewTrap();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                CreateTrap();
            }

            hor = Input.GetAxis("Horizontal");

            ver = Input.GetAxis("Vertical");

            float dirMouse = cine.m_YAxis.m_InputAxisValue;
            //Debug.Log(dirMouse);
            if (dirMouse!=0)
            {
                if (dirMouse < 0)
                    speedRotation = -2;
                else speedRotation = 2;
                Debug.Log("Valor actual de z: "+chest.rotation.eulerAngles.y);
                if (chest.rotation.eulerAngles.z<maxRotate&&chest.rotation.eulerAngles.z>minRotate)
                {
                    
                    Debug.Log("LALALALA");
                    //float change = Mathf.Clamp(chest.rotation.eulerAngles.z + speedper, minRotate, maxRotate);
                    
                    chest.Rotate(0,0,speedRotation);
                }
                
            }
            
            
           // Mathf.Clamp();
            
            movPlayer= new Vector3(hor, 0, ver).normalized;;
            
            float targetAngle = Mathf.Atan2(movPlayer.x, movPlayer.z) * Mathf.Rad2Deg+cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation=Quaternion.Euler(0f,angle,0f);
            
            if (movPlayer.magnitude>=0.1f)
            {
                moveDir = Quaternion.Euler(0, targetAngle, 0f)*Vector3.forward;
            
            }

            speedper = WalkSpeed;
            if (Input.GetKey(RunKey) && characterController.isGrounded)
            {
                speedper = RunSpeed;
            }

            if (Input.GetKeyUp(RunKey))
            {
                speedper = WalkSpeed;
            }

            //moveDir = movPlayer * speed;

            setGravity();
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!cameraChanged)
        {
            characterController.Move(moveDir * speedper * Time.deltaTime);
            moveDir = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
       /* chest.LookAt(Target.position);
        chest.rotation = chest.rotation * Quaternion.Euler(Offset);*/
    }

    void setGravity()
    {
        if (characterController.isGrounded)
        {
            VelCaida = -gravity * Time.deltaTime;
            moveDir.y = VelCaida;
        }
        else
        {
            VelCaida -= gravity * Time.deltaTime;
            moveDir.y = VelCaida;
        }
    }

    void Jump()
    {
        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            VelCaida = jumpForce;
            moveDir.y = VelCaida;
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


    // public void ReceiveDamage(int damage)
    // {
    //     life -= damage;
    //     lifeText.text = life.ToString();
    //     var color = LifeImage.color;
    //     Debug.Log(life / maxLife);
    //     color.a = life / maxLife;
    //     LifeImage.color = color;
    //     if (life <= 0)
    //         GameController.GetInstance().gameOver("KILLED BY X Æ A-12!");
    // }


    private void CreatePreviewTrap()
    {
        _instantiatedPreviewTrap = Instantiate(previewTrap, instantiateTurrets).GetComponent<PreviewTurret>();
    }

    private void UpdatePreviewTrap()
    {
        _turretCanBePlaced = _instantiatedPreviewTrap.isValidPosition();
        _instantiatedPreviewTrap.material.color = _turretCanBePlaced
            ? _instantiatedPreviewTrap.canBePlaced
            : _instantiatedPreviewTrap.canNotBePlaced;
    }

    private void CreateTrap()
    {
        Destroy(_instantiatedPreviewTrap.gameObject);

        if (_turretCanBePlaced && GameController.GetInstance().iron >= 10)
        {
            Entity trap = manager.Instantiate(trapECS);
            var position = instantiateTurrets.position;
            position.y += 0f;
            manager.SetComponentData(trap, new Translation {Value = position});
            manager.SetComponentData(trap, new Rotation {Value = transform.rotation});
            manager.AddBuffer<EnemiesInRange>(trap);
            GameController.GetInstance().UpdateResources(-10);
        }
    }

    private void OnDestroy()
    {
        blobBullet.Dispose();
        blobTrap.Dispose();
    }

    // public void RecoverHealth(int health)
    // {
    //     StopBuffs();
    //     life = Mathf.Min(life + health, maxLife);
    //     lifeText.text = life.ToString();
    // }

    public void IncreaseResources(int resources)
    {
        StopBuffs();
        GameController.GetInstance().UpdateResources(resources);
    }

    public void IncreaseAttack(int Attack)
    {
        StopBuffs();
        damage = initialDamage * Attack;
    }

    public void IncreaseSpeed(int Speed)
    {
        StopBuffs();
        WalkSpeed = this.Speed * Speed;
        RunSpeed = WalkSpeed * 2;
        fireRate = initFireRate / Speed;
        Debug.Log("Speed");
    }

    public void Shotgun(int shotgun)
    {
        StopBuffs();
        this.shotgun = true;
        shotgunRange = shotgun;
        Debug.Log("Shot");
    }

    public void StopBuffs()
    {
        WalkSpeed = this.Speed;
        RunSpeed = WalkSpeed * 2;
        fireRate = initFireRate;
        damage = initialDamage;
        shotgun = false;
    }

    public void ResetToBase()
    {
        characterController.enabled = false;
        transform.position = initialPosition;
        characterController.enabled = true;
    }
}