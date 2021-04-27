using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss Wave Config")]
public class BossWaveConfig : ScriptableObject {

    [Header("Prefab")]
    [SerializeField] GameObject bossPrefab;

    [Header("Wave Info")]
    [SerializeField] float moveSpeed = 2f;

    [Header("Boss Info")]
    [SerializeField] float bossChance = 0.4f;

    public GameObject BossPrefab { get => bossPrefab; set => bossPrefab = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float BossChance { get => bossChance; set => bossChance = value; }
}
