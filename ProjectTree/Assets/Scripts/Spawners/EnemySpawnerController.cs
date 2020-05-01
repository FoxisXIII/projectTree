using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    public EnemySpawner[] _spawners;
    private bool _canSpawn;
    private float _time;

    void Start()
    {
        // _spawners = new EnemySpawner[transform.childCount];
        // var counter = 0;
        // foreach (Transform child in transform)
        //     _spawners[counter] = child.GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_time > .1f)
            SpawnEnemy();
        _time += Time.deltaTime;
    }

    public void SpawnEnemy()
    {
        // if (GameController.GetInstance().CurrentEnemies < 1500)
            _spawners[Random.Range(0, _spawners.Length)].SpawnEnemy();
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }
}