using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using FMOD.Studio;
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

    [Header("Movimiento")] private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
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

    [Header("Shoot")] public GameObject Bullet;
    public GameObject LocFire;
    [Range(0, 1)] public float initFireRate;
    [HideInInspector] public float fireRate;
    private float timer;

    [Header("ECS")] public bool useECS = false;
    private EntityManager manager;
    private Entity bulletEntityPrefab;
    private BlobAssetStore blobBullet;


    [Header("LIFE")] public float maxLife;
    [HideInInspector] public float life;
    public Image lifeImage;
    public TextMeshProUGUI ironText;
    private float aLife;
    public Animator effectDamage;


    [Header("Turrets")] public Transform instantiateTurrets;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;

    [Header("Traps")] public GameObject previewTrap;
    public GameObject trap;
    private PreviewTurret _instantiatedPreviewTrap;
    private Entity trapECS;
    private BlobAssetStore blobTrap;

    //Buffs
    [Header("BUFF")] [HideInInspector] public bool hasBuff;
    [HideInInspector] public Entity buffEntity;
    public int initialDamage;
    [HideInInspector] public int damage;
    private bool shotgun;
    private int shotgunRange;
    public GameObject attackBuffPrefab, shotgunBuffPrefab, speedBuffPrefab;
    private GameObject currentBuff;


    [Header("Change camera")] public GameObject fpsCamera;
    public GameObject birdCamera;
    public Animator hud;
    public KeyCode cameraChange;
    public bool cameraChanged;
    private Vector3 initialPosition;

    [HideInInspector] public string lastAnimatorKey;

    [Header("Intento de vertical")] public Transform chest;
    public float speedRotation;
    public CinemachineFreeLook cine;
    private float minRotate = -1f, maxRotate = 40f;

    [Header("Animaciones")] public Animator anim;
    public HumanBodyBones bones;


    [Header("FMOD paths")] public string jumpSoundPath;
    public string shotSoundPath;
    public string hitSoundPath;
    public string healSoundPath;
    public string idleSoundPath;
    public string dieSoundPath;
    public string cameraTransitionSoundPath;
    public EventInstance idleSoundEvent;

    [Header("Particles")] public GameObject bomb;
    public GameObject shot;
    public GameObject enemyDie;
    public GameObject towerDie;
    private LineRenderer lineRenderer;


    private void Awake()
    {
        GameController.GetInstance().Player = this;
        StopBuffs();

        GameController.GetInstance().Particles = new Dictionary<string, GameObject>()
        {
            {"Bomb", bomb},
            {"Shot", shot},
            {"EnemyDie", enemyDie},
            {"TowerDie", towerDie}
        };
        initialPosition = transform.position;
        lineRenderer = LocFire.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        /* anim = GetComponent<Animator>();
         chest = anim.GetBoneTransform(HumanBodyBones.Chest);*/
        aLife = maxLife;
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
        if (!GameController.GetInstance().GamePaused)
        {
            if (Input.GetKeyDown(cameraChange) && !cameraChanged)
            {
                ChangeCamera();
            }

            if (!cameraChanged)
            {
                timer += Time.deltaTime;
                if (Input.GetMouseButton(0))
                {
                    if (timer >= fireRate)
                    {
                        lineRenderer.enabled = true;
                        GameController.GetInstance().InstantiateParticles("Shot", LocFire.transform.position);
                        if (shotgun)
                            ShotgunECS(LocFire.transform.position, LocFire.transform.forward);
                        else
                            ShootECS(LocFire.transform.position, LocFire.transform.rotation);
                        anim.SetBool("Shoting", true);

                        timer = 0f;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    lineRenderer = LocFire.GetComponent<LineRenderer>();
                    lineRenderer.enabled = false;
                    anim.SetBool("Shoting", false);
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

                movPlayer = new Vector3(hor, 0, ver).normalized;

                float targetAngle = Mathf.Atan2(movPlayer.x, movPlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                    turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                if (movPlayer.magnitude >= 0.1f)
                {
                    moveDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
                }

                if (movPlayer.magnitude >= 0.1f)
                {
                    moveDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
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


                anim.SetFloat("Speed", movPlayer.magnitude >= 0.1f ? speedper : 0f);
                anim.SetBool("onGround", characterController.isGrounded);

                if (movPlayer.Equals(Vector3.zero))
                {
                    if (!SoundManager.GetInstance().IsPlaying(idleSoundEvent))
                    {
                        idleSoundEvent = SoundManager.GetInstance().PlayEvent(idleSoundPath, transform.position, 0.7f);
                    }
                }
                else
                {
                    idleSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }

                setGravity();
                Jump();
            }
        }
        else
        {
            idleSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void FixedUpdate()
    {
        if (!cameraChanged)
        {
            characterController.Move(Time.deltaTime * speedper * moveDir);
            //anim.SetBool("onGround",characterController.isGrounded);
            moveDir = Vector3.zero;
        }
    }


    private void LateUpdate()
    {
        LocFire.transform.forward = cam.transform.forward;

        lineRenderer.SetPosition(0, LocFire.transform.position);
        lineRenderer.SetPosition(1, LocFire.transform.forward * 100 + LocFire.transform.position);
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
            SoundManager.GetInstance().PlayOneShotSound(jumpSoundPath, transform.position);
            anim.SetTrigger("Jump");
        }
    }

    void ShootECS(Vector3 position, Quaternion rotation)
    {
        Entity bullet = manager.Instantiate(bulletEntityPrefab);

        manager.SetComponentData(bullet, new Translation {Value = position});
        manager.SetComponentData(bullet, new Rotation {Value = rotation});
        SoundManager.GetInstance().PlayOneShotSound(shotSoundPath, position);
        var damage = manager.GetComponentData<DealsDamage>(bullet);
        damage.Value = this.damage;
        manager.SetComponentData(bullet, damage);
        var movement = manager.GetComponentData<MovementData>(bullet);
        movement.directionX = LocFire.transform.forward.x;
        movement.directionY = LocFire.transform.forward.y;
        movement.directionZ = LocFire.transform.forward.z;
        manager.SetComponentData(bullet, movement);
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
            tempRot.x = (rotation.x + .05f * x) % 360;
            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + .05f * y) % 360;
                manager.SetComponentData(bullets[index], new Translation {Value = position});
                manager.SetComponentData(bullets[index], new Rotation {Value = Quaternion.Euler(tempRot)});
                var damage = manager.GetComponentData<DealsDamage>(bullets[index]);
                damage.Value = this.damage;
                manager.SetComponentData(bullets[index], damage);
                var ttl = manager.GetComponentData<TimeToLive>(bullets[index]);
                ttl.Value = 2;
                manager.SetComponentData(bullets[index], ttl);
                var movement = manager.GetComponentData<MovementData>(bullets[index]);
                var rot = math.normalize(tempRot);
                movement.directionX = rot.x;
                movement.directionY = rot.y;
                movement.directionZ = rot.z;
                manager.SetComponentData(bullets[index], movement);
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

    public void ReceiveDamage()
    {
        if (life <= 0)
        {
            SoundManager.GetInstance().PlayOneShotSound(dieSoundPath, transform.position);
            GameController.GetInstance().gameOver("KILLED BY X AE A12");
        }
        else
        {
            lifeImage.fillAmount = (float) life / (float) maxLife;
            effectDamage.SetTrigger("Attacked");
            SoundManager.GetInstance().PlayOneShotSound(hitSoundPath, transform.position);
            if (cameraChanged)
            {
                birdCamera.GetComponent<OverviewController>().ChangeCamera();
            }
        }
    }


    private void CreatePreviewTrap()
    {
        _instantiatedPreviewTrap = Instantiate(previewTrap, instantiateTurrets).GetComponent<PreviewTurret>();
    }

    private void UpdatePreviewTrap()
    {
        _turretCanBePlaced = _instantiatedPreviewTrap.isValidPosition();
        _instantiatedPreviewTrap.material.SetColor("_main_color", _turretCanBePlaced
            ? _instantiatedPreviewTrap.canBePlaced
            : _instantiatedPreviewTrap.canNotBePlaced);
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

    public void IncreaseResources(int resources)
    {
        StopBuffs();
        GameController.GetInstance().UpdateResources(resources);
    }

    public void IncreaseAttack(int Attack)
    {
        StopBuffs();
        damage = initialDamage * Attack;
        currentBuff = Instantiate(attackBuffPrefab, transform);
    }

    public void IncreaseSpeed(int Speed)
    {
        StopBuffs();
        WalkSpeed = this.Speed * Speed;
        RunSpeed = WalkSpeed * 2;
        fireRate = initFireRate / Speed;
        currentBuff = Instantiate(speedBuffPrefab, transform);
    }

    public void Shotgun(int shotgun)
    {
        StopBuffs();
        this.shotgun = true;
        shotgunRange = shotgun;
        currentBuff = Instantiate(shotgunBuffPrefab, transform);
    }

    public void StopBuffs()
    {
        WalkSpeed = this.Speed;
        RunSpeed = WalkSpeed * 2;
        fireRate = initFireRate;
        damage = initialDamage;
        shotgun = false;
        Destroy(currentBuff);
    }

    void ChangeCamera()
    {
        birdCamera.transform.position = fpsCamera.transform.position;
        birdCamera.transform.rotation = fpsCamera.transform.rotation;
        idleSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        birdCamera.SetActive(true);
        characterController.enabled = false;
        SoundManager.GetInstance().PlayOneShotSound(cameraTransitionSoundPath, birdCamera.transform.position);
        fpsCamera.SetActive(false);
        //hud.SetBool("towers", true);

        cameraChanged = true;
    }

    public void ResetToBase()
    {
        characterController.enabled = false;
        transform.position = initialPosition;
        characterController.enabled = true;
    }
}