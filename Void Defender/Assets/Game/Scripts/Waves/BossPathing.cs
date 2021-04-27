using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPathing : MonoBehaviour {

    GameSession gameSession;
    BossWaveConfig waveConfig;
    PathConfig pathConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;

    public BossWaveConfig WaveConfig { get => waveConfig; set => waveConfig = value; }
    public PathConfig PathConfig { get => pathConfig; set => pathConfig = value; }

    private void Start() {
        gameSession = FindObjectOfType<GameSession>();
        waypoints = PathConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        float fixedDeltaTime = Time.fixedDeltaTime;
        if (waypointIndex <= waypoints.Count - 1) {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = (WaveConfig.MoveSpeed * fixedDeltaTime) + (WaveConfig.MoveSpeed * EnemySpawner.gameModifier * fixedDeltaTime / 2f);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition) {
                waypointIndex++;
            }
        } else {
            gameSession.Score -= Mathf.Min(gameSession.Score, 1000 + 100 * EnemySpawner.takeawayScoreScaling);
            ResetEnemyPathing();
        }
    }

    private void ResetEnemyPathing() {
        waypointIndex = 0;
        transform.position = waypoints[waypointIndex].transform.position;
    }
}
