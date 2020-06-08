using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
    private EntityManager _manager;
    private List<Entity> turretsToCreate;
    private int _indexToCreate;
    public GameObject position;
    private bool goToPosition, goToCharacter;

    [Header("FMOD")]
    public string turretCollocationSoundPath;
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
        goToPosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraChange) && GameController.GetInstance().Player.cameraChanged)
        {
            ChangeCamera();
        }

        if (goToPosition)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, position.transform.position, 15 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, position.transform.rotation, Time.deltaTime);

            if (transform.position == position.transform.position)
                goToPosition = false;
        }

        if (goToCharacter)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                GameController.GetInstance().Player.fpsCamera.transform.position, 100 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                GameController.GetInstance().Player.fpsCamera.transform.rotation, Time.deltaTime);

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
            for (int i = 1; i < turretsToCreate.Count + 1; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    _indexToCreate = i - 1;
                    CreatePreviewTurret();
                    _creating = true;
                }
            }

            if (_creating)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CreateTurret(_indexToCreate);
                    _creating = false;
                }

                UpdatePreviewTurret();
            }
        }
    }

    public void ChangeCamera()
    {
        goToPosition = false;
        goToCharacter = true;
        GameController.GetInstance().Player.hud.SetBool("towers", false);
        Cursor.visible = false;
        SoundManager.GetInstance().PlayOneShotSound(cameraTransitionSoundPath, transform.position);

        if (_instantiatedPreviewTurret != null)
        {
            Destroy(_instantiatedPreviewTurret);
        }
    }

    private void CreateTurret(int index)
    {
        Destroy(_instantiatedPreviewTurret.gameObject);
        if (_turretCanBePlaced && GameController.GetInstance().iron >= 20)
        {
            Entity turret = _manager.Instantiate(turretsToCreate[index]);
            var position = _instantiatedPreviewTurret.gameObject.transform.position;
            _manager.SetComponentData(turret, new Translation {Value = position});

            GameController.GetInstance().UpdateResources(-20);
            GameController.GetInstance().TowersPlaced++;

            // _manager.AddBuffer<EnemiesInRange>(turret);
            // _manager.AddBuffer<TurretsInRange>(turret);
            _manager.AddComponent(turret, typeof(TurretFMODPaths));
            _manager.SetComponentData(turret, new TurretFMODPaths
            {
                ShotPath = turretShotSoundPath,
                DestroyPath = turretDestroySoundPath,
                AuraPath = turretAuraSoundPath,
                HealPath = turretHealSoundPath,
                BuffPath = turretBuffSoundPath
            });
            SoundManager.GetInstance().PlayOneShotSound(turretCollocationSoundPath, transform);
        }
    }

    private void UpdatePreviewTurret()
    {
        _turretCanBePlaced = _instantiatedPreviewTurret.isValidPosition();
        _instantiatedPreviewTurret.material.color = _turretCanBePlaced
            ? _instantiatedPreviewTurret.canBePlaced
            : _instantiatedPreviewTurret.canNotBePlaced;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var gridPosition = grid.GetNearestpointOnGrid(hit.point);
            if (!gridPosition.Equals(Vector3.zero))
            {
                gridPosition.y = hit.point.y;
                _instantiatedPreviewTurret.gameObject.transform.position = gridPosition;
            }
        }
    }

    private void CreatePreviewTurret()
    {
        if (_instantiatedPreviewTurret != null)
            Destroy(_instantiatedPreviewTurret.gameObject);

        _instantiatedPreviewTurret = Instantiate(previewTurret).GetComponent<PreviewTurret>();
        if (_indexToCreate == 0 || _indexToCreate == 1)
            _instantiatedPreviewTurret.transform.GetChild(0).localScale *= 2;
    }

    public void OnClick(int index)
    {
        CreatePreviewTurret();
        _creating = true;
        _indexToCreate = index;
    }

    private void OnDestroy()
    {
        blobTurret.Dispose();
    }
}