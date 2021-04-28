using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [Header("Wave Configs")]
    [SerializeField] List<PathConfig> pathConfigs;
    [SerializeField] List<EnemyWaveConfig> enemyConfigs;
    [SerializeField] List<BossWaveConfig> bossConfigs;

    [Header("Game Info")]
    [SerializeField] bool looping = true;

    [Header("SFX")]
    [SerializeField] AudioClip bossWaveSFX;
    [SerializeField] [Range(0, 1)] float bossWaveSFXVolume = 0.33f;


    // Controls which wave gets spawned
    private List<EnemyWaveConfig> recentEnemies;
    private List<PathConfig> recentPaths;
    private int nextBossWave;

    // Game modifications based on wave information
    private int currentWaveNumber = 0;
    public static int pointMultiplier = 1;
    public static float gameModifier = 0f;
    public static int takeawayScoreScaling = 0;

    // Parent GameObject - A place to store all instantiated objects in the unity hierarchy
    GameObject enemyParent;
    private const string ENEMY_PARENT_NAME = "EnemySpawner";


    private IEnumerator Start() {
        CreateEnemyParent();
        InitialSetup();
        do {
            yield return StartCoroutine(SpawnWave());
        } while (looping);
    }

    private void CreateEnemyParent() {
        enemyParent = GameObject.Find(ENEMY_PARENT_NAME);
        if (!enemyParent) {
            enemyParent = new GameObject(ENEMY_PARENT_NAME);
        }
    }

    private void InitialSetup() {
        recentEnemies = new List<EnemyWaveConfig>();
        recentPaths = new List<PathConfig>();
        SetNextBossWave();
    }

    private IEnumerator SpawnWave() {
        // Debug.Log("Next Boss Wave: " + nextBossWave + "  -  Current Wave: " + (currentWaveNumber + 1));
        if (currentWaveNumber != nextBossWave) {
            yield return StartCoroutine(SpawnEnemyWave());
        } else {
            yield return StartCoroutine(SpawnBossWave());
            SetNextBossWave();
        }
        WaveCountUpdates();
        if (currentWaveNumber % 25 == 0) {
            pointMultiplier++;
        }
    }

    private IEnumerator SpawnEnemyWave() {
        EnemyWaveConfig enemyWaveConfig = EnsureNewEnemy();
        PathConfig pathConfig = EnsureNewPath();
        // Debug.Log("Recent: " + PrintRecent());

        int numEnemies = enemyWaveConfig.NumberOfEnemies + Random.Range(0, 4);
        for (int enemyCount = 0; enemyCount < numEnemies; enemyCount++) {
            GetNewEnemy(enemyWaveConfig, pathConfig);
            yield return new WaitForSeconds(enemyWaveConfig.TimeBetweenSpawns);
        }
    }

    private EnemyWaveConfig EnsureNewEnemy() {
        EnemyWaveConfig enemyWaveConfig;
        do {
            enemyWaveConfig = GetNextEnemyConfig();
        } while (recentEnemies.Contains(enemyWaveConfig));
        recentEnemies.Add(enemyWaveConfig);
        return enemyWaveConfig;
    }

    private PathConfig EnsureNewPath() {
        PathConfig pathConfig;
        do {
            pathConfig = GetNextPathConfig(false);
        } while (recentPaths.Contains(pathConfig));
        recentPaths.Add(pathConfig);
        return pathConfig;
    }

    private EnemyWaveConfig GetNextEnemyConfig() {
        EnemyWaveConfig enemyWaveConfig = null;

        float runningTotal = 0f;
        float random = Random.value;
        foreach (EnemyWaveConfig config in enemyConfigs) {
            runningTotal += GetNextEnemyChance(config);
            if (random < runningTotal) {
                enemyWaveConfig = config;
                if (recentEnemies.Contains(enemyWaveConfig)) {
                    continue;
                }
                break;
            }
        }
        return enemyWaveConfig;
    }

    private float GetNextEnemyChance(EnemyWaveConfig enemyWaveConfig) {
        float currentChanceDelta = enemyWaveConfig.ChanceDelta;
        float currentChanceModifier = Mathf.Min(1, currentWaveNumber / 100f);
        float currentChance = enemyWaveConfig.StartingChance + currentChanceDelta * currentChanceModifier;
        return currentChance;
    }

    private GameObject GetNewEnemy(EnemyWaveConfig enemyWaveConfig, PathConfig pathConfig) {
        var enemy = Instantiate(enemyWaveConfig.EnemyPrefab, pathConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
        var enemyPathing = enemy.GetComponent<EnemyPathing>();
        enemyPathing.WaveConfig = enemyWaveConfig;
        enemyPathing.PathConfig = pathConfig;
        enemy.transform.parent = enemyParent.transform;
        return enemy;
    }

    private IEnumerator SpawnBossWave() {
        int numEnemies;
        do {
            numEnemies = FindObjectsOfType<Enemy>().Length;
            yield return null;
        } while (numEnemies > 0);
        FindObjectOfType<MusicPlayer>().PlayOneShot(bossWaveSFX, bossWaveSFXVolume);

        BossWaveConfig bossWaveConfig = GetNextBossConfig();
        PathConfig pathConfig = GetNextPathConfig(true);
        var boss = GetNewBoss(bossWaveConfig, pathConfig);
        while (boss) {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }

    private BossWaveConfig GetNextBossConfig() {
        BossWaveConfig bossWaveConfig = null;

        float runningTotal = 0f;
        float random = Random.value;
        foreach (BossWaveConfig config in bossConfigs) {
            runningTotal += config.BossChance;
            if (random < runningTotal) {
                bossWaveConfig = config;
                break;
            }
        }
        return bossWaveConfig;
    }

    private GameObject GetNewBoss(BossWaveConfig bossWaveConfig, PathConfig pathConfig) {
        var boss = Instantiate(bossWaveConfig.BossPrefab, pathConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
        var bossPathing = boss.GetComponent<BossPathing>();
        bossPathing.WaveConfig = bossWaveConfig;
        bossPathing.PathConfig = pathConfig;
        boss.transform.parent = enemyParent.transform;
        return boss;
    }

    private PathConfig GetNextPathConfig(bool isBoss) {
        PathConfig pathConfig = null;
        List<PathConfig> validPaths = new List<PathConfig>();

        foreach (PathConfig config in pathConfigs) {
            if (isBoss && config.BossPath) {
                validPaths.Add(config);
            } else if (!isBoss && !config.BossPath) {
                validPaths.Add(config);
            }
        }

        pathConfig = validPaths[Random.Range(0, validPaths.Count)];
        return pathConfig;
    }

    private void SetNextBossWave() {
        float gameLengthModifier = Mathf.Min(1, currentWaveNumber / 100f);
        int baseBetweenBossWaves = 30; // 30
        int minimumBetweenBossWaves = 15; // 15
        int wave = baseBetweenBossWaves - Mathf.FloorToInt(minimumBetweenBossWaves * gameLengthModifier);
        nextBossWave += Random.Range(wave - 3, wave + 4);
        // nextBossWave += 1;
    }

    private void WaveCountUpdates() {
        currentWaveNumber++;
        gameModifier = currentWaveNumber * 0.01f;
        takeawayScoreScaling = Mathf.CeilToInt(gameModifier * 10);
        if (recentEnemies.Count > 2) {
            recentEnemies.RemoveAt(0);
        }
        if (recentPaths.Count > 2) {
            recentPaths.RemoveAt(0);
        }
    }

    private string PrintRecent() {
        string temp = "";
        for (int i = 0; i < recentEnemies.Count; i++) {
            temp += recentEnemies[i];
            temp += " - ";
            temp += recentPaths[i];
            temp += "  |  ";
        }
        return temp;
    }
}
