using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {

    [SerializeField] float speedOfSpin = 360f;
    [SerializeField] float growthRate = 1f;
    Player player;

    private void Start() {
        player = FindObjectOfType<Player>();
        if (player && (gameObject.tag == "Bomb" || gameObject.tag == "Jacks" || gameObject.tag == "Blast")) {
            RotateToFacePlayer();
        }
    }

    private void Update() {
        transform.Rotate(0, 0, speedOfSpin * Time.deltaTime);
        if (gameObject.tag == "Blast") {
            GrowOverTime();
        }
        if (player && (gameObject.tag == "Jacks" || gameObject.tag == "Fireball")) {
            RotateToFacePlayer();
        }
    }

    private void RotateToFacePlayer() {
        Vector3 target = player.transform.position;
        target.z = 0f;
        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        angle += 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void GrowOverTime() {
        float growthThisFrame = growthRate * Time.deltaTime;
        transform.localScale += new Vector3(growthThisFrame, growthThisFrame, 0);
    }
}
