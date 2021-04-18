using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [Header("Waves")]
    [SerializeField] int startingWave = 0;
    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] bool looping = false;

    [Header("SFX")]
    [SerializeField] AudioClip bossWaveSFX;
    [SerializeField] [Range(0, 1)] float bossWaveSFXVolume = 0.25f;

    private int waveCounter = 0;
    public static int loopCounter = 1;
    public static float gameModifier = 0f;
    public static int takeawayScoreScaling = 0;

    // Start is called before the first frame update
    private IEnumerator Start() {
        ShuffleWave(waveConfigs);
        do {
            yield return StartCoroutine(SpawnAllWaves());
            loopCounter++;
        } while (looping);
    }

    private IEnumerator SpawnAllWaves() {
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++) {
            WaveCountMods();
            var currentWave = waveConfigs[waveIndex];
            if (currentWave.BossWave) {
                yield return StartCoroutine(SpawnBossWave(currentWave));
            } else {
                yield return StartCoroutine(SpawnWave(currentWave));
            }
        }
    }

    private void WaveCountMods() {
        waveCounter++;
        gameModifier = waveCounter * 0.01f;
        takeawayScoreScaling = Mathf.CeilToInt(gameModifier * 5);
    }

    private IEnumerator SpawnWave(WaveConfig waveConfig) {
        int numEnemies = waveConfig.NumberOfEnemies + Random.Range(0, 4);
        for (int enemyCount = 0; enemyCount < numEnemies; enemyCount++) {
            GetNewEnemy(waveConfig);
            yield return new WaitForSeconds(waveConfig.TimeBetweenSpawns); // + Random.Range(0, waveConfig.SpawnRandomFactor));
        }
    }

    private IEnumerator SpawnBossWave(WaveConfig waveConfig) {
        int numEnemies = FindObjectsOfType<Enemy>().Length;
        while (numEnemies > 0) {
            numEnemies = FindObjectsOfType<Enemy>().Length;
            yield return null;
        }
        FindObjectOfType<MusicPlayer>().PlayOneShot(bossWaveSFX, bossWaveSFXVolume);
        var boss = GetNewEnemy(waveConfig);
        while (boss) {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }

    private GameObject GetNewEnemy(WaveConfig waveConfig) {
        var enemy = Instantiate(waveConfig.EnemyPrefab, waveConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
        enemy.GetComponent<EnemyPathing>().WaveConfig = waveConfig;
        return enemy;
    }

    private void ShuffleWave(List<WaveConfig> list) {
        int bucketSize = 5;
        List<WaveConfig> segment = new List<WaveConfig>();
        for (int i = 0; i < list.Count; i++) {
            if (i > 0 && i % bucketSize == 0) {
                segment = new List<WaveConfig>();
            }
            segment.Add(list[i]);
            if (i % bucketSize == bucketSize - 1) {
                ShuffleWaveSegment(segment, list, i);
            }
        }
    }

    private void ShuffleWaveSegment(List<WaveConfig> segment, List<WaveConfig> list, int index) {
        for (int i = 0; i < segment.Count; i++) {
            WaveConfig temp = segment[i];
            int randomIndex = Random.Range(i, segment.Count);
            segment[i] = segment[randomIndex];
            segment[randomIndex] = temp;
        }
        for (int i = 0; i < segment.Count; i++) {
            list[index - i] = segment[i];
        }
    }
}
