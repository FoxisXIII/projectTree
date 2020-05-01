using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DefenseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject defensePrefab;
    private EntityManager _entityManager;
    private Entity _defenseEntityPrefab;
    private BlobAssetStore blob;
    
    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blob = new BlobAssetStore();
        _defenseEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(defensePrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,  blob));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SpawnDefense();
    }
    
    private void SpawnDefense()
    {
        Entity enemy = _entityManager.Instantiate(_defenseEntityPrefab);
        _entityManager.SetComponentData(enemy, new Translation() {Value = transform.position});
        _entityManager.SetComponentData(enemy, new Rotation() {Value = Quaternion.identity});
    }

    private void OnDestroy()
    {
        blob.Dispose();
    }
}
