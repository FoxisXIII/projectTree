using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Transforms;
using UnityEditor.Experimental.GraphView;
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
    public GameObject toCreateText;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraChange) /*|| GameController.GetInstance().WaveInProcess*/)
        {
            GameController.GetInstance().Player.characterController.enabled = true;
            GameController.GetInstance().Player.fpsCamera.SetActive(true);
            GameController.GetInstance().Player.cameraChanged = false;
            Cursor.visible = false;
            if (!ReferenceEquals(_instantiatedPreviewTurret, null))
            {
                Destroy(_instantiatedPreviewTurret);
            }
            gameObject.SetActive(false);
        }

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
            if (Input.GetMouseButtonDown(1))
            {
                CreateTurret(_indexToCreate);
                _creating = false;
            }

            UpdatePreviewTurret();
        }
    }

    private void CreateTurret(int index)
    {
        Destroy(_instantiatedPreviewTurret.gameObject);
        toCreateText.SetActive(false);

        if (_turretCanBePlaced && GameController.GetInstance().RecursosA >= 20)
        {
            Entity turret = _manager.Instantiate(turretsToCreate[index]);
            var position = _instantiatedPreviewTurret.gameObject.transform.position;
            _manager.SetComponentData(turret, new Translation {Value = position});
            _manager.AddBuffer<EnemiesInRange>(turret);
            _manager.AddBuffer<TurretsInRange>(turret);
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
            var gridPosition = grid.GetNearestpointOnGrid(hit.point);
            if (!gridPosition.Equals(Vector3.zero))
            {
                gridPosition.y += 1f;
                _instantiatedPreviewTurret.gameObject.transform.position = gridPosition;
            }
        }
    }

    private void CreatePreviewTurret()
    {
        if (!ReferenceEquals(_instantiatedPreviewTurret, null))
        {
            //Destroy(_instantiatedPreviewTurret.gameObject);
            Destroy(_instantiatedPreviewTurret);
        }
        _instantiatedPreviewTurret = Instantiate(previewTurret).GetComponent<PreviewTurret>();
        toCreateText.SetActive(true);
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