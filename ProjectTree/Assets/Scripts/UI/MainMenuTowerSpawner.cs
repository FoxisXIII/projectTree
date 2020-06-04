using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MainMenuTowerSpawner : MonoBehaviour
{
    public GameObject tower;

    public Vector3 position;
    public string turretShotSoundPath;

    // Start is called before the first frame update
    void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var blobAsset = new BlobAssetStore();
        Entity turret = manager.Instantiate(GameObjectConversionUtility.ConvertGameObjectHierarchy(tower, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAsset)));
        manager.SetComponentData(turret, new Translation {Value = this.position});

        GameController.GetInstance().UpdateResources(-20);
        GameController.GetInstance().TowersPlaced++;

        manager.AddBuffer<EnemiesInRange>(turret);
        manager.AddBuffer<TurretsInRange>(turret);
        manager.AddComponent(turret, typeof(TurretFMODPaths));
        manager.SetComponentData(turret, new TurretFMODPaths
        {
            ShotPath = turretShotSoundPath,
        });
        blobAsset.Dispose();
    }
}