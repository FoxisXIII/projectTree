using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = FMOD.Debug;

public class OverviewController : MonoBehaviour
{
    public KeyCode cameraChange;
    private Camera _camera;
    public Grid grid;

    public GameObject previewTurret;
    public GameObject[] prefabsTurrets;
    private bool _creating;
    private BlobAssetStore blobTurret;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;
    public GameObject PopupTextObject;
    private EntityManager _manager;
    private List<Entity> turretsToCreate;
    private int _indexToCreate;
    private GameObject _placeToCreate;
    public GameObject position;
    private bool goToPosition, goToCharacter;
    public TurretSpots spotManager;
    public GameObject TurretHUD;
    public int camMovSpeed;
    public int camRotSpeed;

    [Header("FMOD")] public string turretCollocationSoundPath;
    public string turretShotSoundPath;
    public string turretBombSoundPath;
    public string turretAuraSoundPath;
    public string turretDestroySoundPath;
    public string turretHealSoundPath;
    public string turretBuffSoundPath;
    public string cameraTransitionSoundPath;

    // Start is called before the first frame update
    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobTurret = new BlobAssetStore();
        turretsToCreate = new List<Entity>();
        for (int i = 0; i < prefabsTurrets.Length; i++)
        {
            turretsToCreate.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabsTurrets[i],
                GameObjectConversionSettings.FromWorld(_manager.World, blobTurret)));
        }

        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        spotManager.EnableParticles();
        goToPosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraChange) && GameController.GetInstance().Player.cameraChanged)
        {
            ChangeCamera();
        }

        camMovSpeed = 100;
        if (goToPosition)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, position.transform.position, camMovSpeed * Time.deltaTime);
            camRotSpeed = 8;
            transform.rotation = Quaternion.Lerp(transform.rotation, position.transform.rotation, camRotSpeed*Time.deltaTime);

            if (transform.position == position.transform.position)
                goToPosition = false;
        }

        if (goToCharacter)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                GameController.GetInstance().Player.fpsCamera.transform.position, camMovSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                GameController.GetInstance().Player.fpsCamera.transform.rotation, camRotSpeed*Time.deltaTime);

            if (transform.position == GameController.GetInstance().Player.fpsCamera.transform.position)
            {
                goToCharacter = false;
                GameController.GetInstance().Player.characterController.enabled = true;
                GameController.GetInstance().Player.fpsCamera.SetActive(true);
                GameController.GetInstance().Player.cameraChanged = false;
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (_creating)
            {
                for (int i = 1; i < turretsToCreate.Count + 1; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        _indexToCreate = i - 1;
                        CreateTurret();
                        _creating = false;
                        TurretHUD.SetActive(false);
                        break;
                    }
                }
            }
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("TurretSpot") && Input.GetMouseButtonDown(0))
                {
                    _creating = true;
                    _placeToCreate = hit.collider.gameObject;
                    TurretHUD.SetActive(true);
                }
            }
            
        }
    }

    public void ChangeCamera()
    {
        _creating = false;
        spotManager.DisableParticles();
        goToPosition = false;
        goToCharacter = true;
        TurretHUD.SetActive(false);
        Cursor.visible = false;
        SoundManager.GetInstance().PlayOneShotSound(cameraTransitionSoundPath, transform.position);
    }

    private void CreateTurret()
    {
        GameController.GetInstance().Player.hud.SetBool("towers", false);
        CreatingSpot spot = _placeToCreate.GetComponent<CreatingSpot>();
        if (GameController.GetInstance().iron >= 20 && !spot.HasTurret)
        {
            Entity turret = _manager.Instantiate(turretsToCreate[_indexToCreate]);
            _manager.SetComponentData(turret, new Translation {Value = _placeToCreate.transform.position});
            spot.AddTurret(turret);
            GameController.GetInstance().UpdateResources(-20);
            GameController.GetInstance().TowersPlaced++;
            _manager.AddComponent(turret, typeof(TurretFMODPaths));
            _manager.SetComponentData(turret, new TurretFMODPaths
            {
                ShotPath = turretShotSoundPath,
                HealPath = turretHealSoundPath,
                BuffPath = turretBuffSoundPath
            });
            SoundManager.GetInstance().PlayOneShotSound(turretCollocationSoundPath, transform.position);
            if (_indexToCreate >= 2)
            {
                SoundManager.GetInstance().PlayOneShotSound(turretAuraSoundPath, turret);
            }
        }
        else
        {
            PopupTextObject.SetActive(true);
            PopupText popupText = PopupTextObject.GetComponent<PopupText>();
            if (GameController.GetInstance().iron < 20)
            {
                popupText.Setup("You don't have enough iron");
            }
            else
            {
                popupText.Setup("There's already a turret there");
            }
        }
    }

    public void OnClick(int index)
    {
        print("clicked");
        _indexToCreate = index-1;
        _creating = false;
        CreateTurret();
    }

    private void OnDestroy()
    {
        blobTurret.Dispose();
    }
}