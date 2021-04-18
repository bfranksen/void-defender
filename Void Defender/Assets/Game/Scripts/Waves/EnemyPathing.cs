using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour {

    GameSession gameSession;
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    bool leaked = false;

    // Use this for initialization
    private void Start() {
        gameSession = FindObjectOfType<GameSession>();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        float fixedDeltaTime = Time.fixedDeltaTime;
        if (waypointIndex <= waypoints.Count - 1) {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = (waveConfig.MoveSpeed * fixedDeltaTime) + (waveConfig.MoveSpeed * EnemySpawner.gameModifier * fixedDeltaTime / 2f);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition) {
                waypointIndex++;
            }
        } else if (!leaked) {
            leaked = true;
            ResetEnemyPathing();
        } else if (waveConfig.BossWave) {
            gameSession.Score -= Mathf.Min(gameSession.Score, 1000 + 100 * EnemySpawner.takeawayScoreScaling);
            ResetEnemyPathing();
        } else {
            Destroy(gameObject);
            gameSession.Score -= Mathf.Min(gameSession.Score, 500 + 50 * EnemySpawner.takeawayScoreScaling);
        }
    }

    private void ResetEnemyPathing() {
        waypointIndex = 0;
        transform.position = waypoints[waypointIndex].transform.position;
    }

    public WaveConfig WaveConfig { get => waveConfig; set => waveConfig = value; }
}
