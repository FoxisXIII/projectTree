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

    private int bossInScenario;


    [SerializeField] private TextMeshProUGUI nextRoundTimeText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Image currentEnemiesImage;
    [SerializeField] private Animator hud;
    private float spawnEnemyTime;
    private bool[] hordes;

    private float timeToNextRound;

    void Awake()
    {
        GameController.GetInstance().MaxWaveEnemies = maxWaveEnemies;
        GameController.GetInstance().EnemiesSpawnRate = enemySpawnRate;
        hordes = new bool[spawners.Length];
        resetHordes();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _canEndWave = true;
    }

    private void resetHordes()
    {
        for (int i = 0; i < hordes.Length; i++)
            hordes[i] = false;
    }

    private void Start()
    {
        GameController.GetInstance().WaveCounter = 0;
        GameController.GetInstance().iron = 60;
        EndWave();
    }

    // Update is called once per frame
    void Update()
    {
        StartWave();

        if (GameController.GetInstance().NormalWave)
            SpawnEnemy();
        else if (GameController.GetInstance().BossWave)
            SpawnBoss();

        EndWave();

        if ((GameController.GetInstance().MaxWaveEnemies - GameController.GetInstance().DiedEnemies) <= 0)

            nextRoundTime += 5 * Time.deltaTime;
        else
            nextRoundTime += Time.deltaTime;
        spawnEnemyTime += Time.deltaTime;

        nextRoundTimeText.SetText(Math.Max(Math.Round((Decimal) (waveCooldown - nextRoundTime), 0), 0).ToString());
    }

    private void StartWave()
    {
        if ( /*!_canSpawn && */nextRoundTime >= waveCooldown)
        {
            GameController.GetInstance().startWave();

            hud.SetBool("inRound", true);
            hud.SetBool("nextRound", false);

            roundText.SetText("ROUND " + GameController.GetInstance().WaveCounter);
            resetHordes();
            if (GameController.GetInstance().WaveCounter > 2)
            {
                var maxHorde = Mathf.Min(hordes.Length, 1 + (1 * (GameController.GetInstance().WaveCounter / 15)));
                var currentHorde = 0;
                for (int i = 0; i < hordes.Length; i++)
                {
                    if (Random.Range(0f, 1f) < .25f)
                    {
                        hordes[i] = true;
                        currentHorde++;
                    }
                    else
                        hordes[i] = false;

                    if (currentHorde == maxHorde)
                        break;
                }
            }

            nextRoundTime = 0;
            bossInScenario = 0;
            spawnEnemyTime = 0;
            Debug.Log(GameController.GetInstance().MaxWaveEnemies +" - "+GameController.GetInstance().MaxWaveEnemies);
            
            waveCooldown = GameController.GetInstance().EnemiesSpawnRate * GameController.GetInstance().MaxWaveEnemies +
                           60f;
            if (GameController.GetInstance().BossWave)
                waveCooldown += GameController.GetInstance().NumberOfBoses *
                                GameController.GetInstance().EnemiesSpawnRate * 10f;
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
        }
    }

    public void SpawnEnemy()
    {
        if (spawnEnemyTime >= GameController.GetInstance().EnemiesSpawnRate &&
            GameController.GetInstance().CurrentEnemies <
            GameController.GetInstance().MaxWaveEnemies)
        {
            var random = Random.Range(0, spawners.Length);
            spawners[random].SpawnEnemy(hordes[random]);

            spawnEnemyTime = 0;
        }

        ChangeUiValues();
    }

    public void SpawnBoss()
    {
        if (spawnEnemyTime >= GameController.GetInstance().EnemiesSpawnRate)
        {
            if (bossInScenario < GameController.GetInstance().NumberOfBoses)
            {
                spawners[Random.Range(0, spawners.Length)].SpawnBoss();
                bossInScenario++;
            }
            else if (GameController.GetInstance().CurrentEnemies < GameController.GetInstance().MaxWaveEnemies)
            {
                var random = Random.Range(0, spawners.Length);
                spawners[random].SpawnEnemy(hordes[random]);
            }

            spawnEnemyTime = 0;
        }

        ChangeUiValues();
    }

    private void ChangeUiValues()
    {
        currentEnemiesImage.fillAmount = nextRoundTime / waveCooldown;
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }

    public void Dispose()
    {
        foreach (var spawner in spawners)
        {
            spawner.Dispose();
        }
    }
}