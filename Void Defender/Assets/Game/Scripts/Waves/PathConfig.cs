using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Path Config")]
public class PathConfig : ScriptableObject {

    [Header("Prefab")]
    [SerializeField] GameObject pathPrefab;

    [Header("Path Info")]
    [SerializeField] bool bossPath = false;

    public bool BossPath { get => bossPath; set => bossPath = value; }

    public List<Transform> GetWaypoints() {
        var waveWaypoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform) {
            waveWaypoints.Add(child);
        }
        return waveWaypoints;
    }
}
