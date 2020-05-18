using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class OverviewController : MonoBehaviour
{
    public KeyCode cameraChange;
    private Camera _camera;
    //private Grid _grid;

    public GameObject previewTurret;
    public GameObject shootingTurret;
    private bool _creating;
    private BlobAssetStore blobTurret;
    private Entity turretECS;
    private PreviewTurret _instantiatedPreviewTurret;
    private bool _turretCanBePlaced;
    private EntityManager _manager;
    
    // Start is called before the first frame update
    void Start()
    {
        //_grid = new Grid(20,10, 10f);
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobTurret = new BlobAssetStore();
        turretECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(shootingTurret,
            GameObjectConversionSettings.FromWorld(_manager.World, blobTurret));
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraChange) || GameController.GetInstance().WaveInProcess)
        {
            GameController.GetInstance().Player.characterController.enabled = true;
            GameController.GetInstance().Player.fpsCamera.SetActive(true);
            GameController.GetInstance().Player.cameraChanged = false;
            Cursor.visible = false;
            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreatePreviewTurret();
            _creating = true;
        }
        if (_creating)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CreateTurret();
                _creating = false;
            }
            UpdatePreviewTurret();
        }

    }

    private void CreateTurret()
    {
        Destroy(_instantiatedPreviewTurret.gameObject);

        if (_turretCanBePlaced && GameController.GetInstance().RecursosA>=20)
        {
            Entity turret = _manager.Instantiate(turretECS);
            var position = _instantiatedPreviewTurret.gameObject.transform.position;
            _manager.SetComponentData(turret, new Translation {Value = position});
            _manager.AddBuffer<EnemiesInRange>(turret);
            GameController.GetInstance().UpdateResources(-20);
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
            _instantiatedPreviewTurret.gameObject.transform.position = hit.point;
            Vector3 position = _instantiatedPreviewTurret.gameObject.transform.position;
            position.y+=.5f;
            _instantiatedPreviewTurret.gameObject.transform.position = position;
            _instantiatedPreviewTurret.gameObject.transform.rotation =
                Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    private void CreatePreviewTurret()
    {
        _instantiatedPreviewTurret = Instantiate(previewTurret).GetComponent<PreviewTurret>();
    }

    private void OnDestroy()
    {
        blobTurret.Dispose();
    }
}
