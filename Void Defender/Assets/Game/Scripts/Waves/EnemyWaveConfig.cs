using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class EnemyWaveConfig : ScriptableObject {

    [Header("Prefab")]
    [SerializeField] GameObject enemyPrefab;

    [Header("Wave Info")]
    [SerializeField] float timeBetweenSpawns = 0.75f;
    [SerializeField] float spawnRandomFactor = 0.5f;
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] float moveSpeed = 2f;

    [Header("Enemy Info")]
    [SerializeField] float startingChance = 0.2f;
    [SerializeField] float finalChance = 0.2f;
    [SerializeField] float chanceDelta = 0.0f;

    public GameObject EnemyPrefab { get => enemyPrefab; set => enemyPrefab = value; }
    public float TimeBetweenSpawns { get => timeBetweenSpawns; set => timeBetweenSpawns = value; }
    public float SpawnRandomFactor { get => spawnRandomFactor; set => spawnRandomFactor = value; }
    public int NumberOfEnemies { get => numberOfEnemies; set => numberOfEnemies = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float StartingChance { get => startingChance; set => startingChance = value; }
    public float FinalChance { get => finalChance; set => finalChance = value; }
    public float ChanceDelta { get => chanceDelta; set => chanceDelta = value; }
}
