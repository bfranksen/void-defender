using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour {

    [SerializeField] float moveSpeed = 2.5f;

    Player player;

    private void Start() {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    private void Update() {
        if (player) {
            Move();
        }
    }

    private void Move() {
        var targetPosition = player.transform.position;
        var movementThisFrame = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
    }
}
