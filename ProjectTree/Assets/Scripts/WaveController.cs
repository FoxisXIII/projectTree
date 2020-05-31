using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    public EnemySpawner[] spawners;

    private bool _canSpawn, _canEndWave;
    private float nextRoundTime;
    public float waveCooldown;
    public float enemySpawnRate;
    public int maxWaveEnemies;

    public Material[] flyAnim;
    public Material[] groundAnim;

    [SerializeField] private TextMeshProUGUI nextRoundTimeText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Image currentEnemiesImage;
    [SerializeField] private Animator hud;
    [SerializeField] private float spawnEnemyTime;

    void Awake()
    {
        GameController.GetInstance().MaxWaveEnemies = maxWaveEnemies;
        GameController.GetInstance().EnemiesSpawnRate = enemySpawnRate;
        GameController.GetInstance().EnemiesKilled = 0;

        Dictionary<String, List<Material>> dict = new Dictionary<string, List<Material>>();
        dict.Add("Dron", new List<Material>(flyAnim));
        dict.Add("Tank", new List<Material>(groundAnim));
        GameController.GetInstance().setMaterials(dict);

        _canEndWave = true;
    }

    private void Start()
    {
        GameController.GetInstance().WaveCounter = 0;
        EndWave();
    }

    // Update is called once per frame
    void Update()
    {
        StartWave();

        SpawnEnemy();

        EndWave();

        nextRoundTime += Time.deltaTime;
        spawnEnemyTime += Time.deltaTime;

        nextRoundTimeText.SetText(Math.Max(Math.Round((Decimal) (waveCooldown - nextRoundTime), 0), 0).ToString());
    }

    private void StartWave()
    {
        if (!_canSpawn && nextRoundTime >= waveCooldown)
        {
            GameController.GetInstance().startWave();
            hud.SetBool("inRound", true);
            hud.SetBool("nextRound", false);
            roundText.SetText("ROUND " + GameController.GetInstance().WaveCounter);
            _canSpawn = true;
        }
    }

    private void EndWave()
    {
        if (_canEndWave)
        {
            GameController.GetInstance().endWave();
            hud.SetBool("inRound", false);
            hud.SetBool("nextRound", true);
            _canEndWave = false;
            _canSpawn = false;
            nextRoundTime = 0;
        }
    }

    public void SpawnEnemy()
    {
        if (_canSpawn && !_canEndWave && spawnEnemyTime >= GameController.GetInstance().EnemiesSpawnRate &&
            GameController.GetInstance().CurrentEnemies <
            GameController.GetInstance().MaxWaveEnemies)
        {
            spawners[Random.Range(0, spawners.Length)].SpawnEnemy();

            spawnEnemyTime = 0;
        }

        var waveEnemies = GameController.GetInstance().MaxWaveEnemies - GameController.GetInstance().DiedEnemies;

        currentEnemiesImage.fillAmount =
            1f - ((float) waveEnemies / (float) GameController.GetInstance().MaxWaveEnemies);
        _canEndWave = GameController.GetInstance().DiedEnemies >= GameController.GetInstance().MaxWaveEnemies;
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }
}