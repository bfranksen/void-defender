using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {

    [SerializeField] float speedOfSpin = 360f;

    private void Start() {
        if (gameObject.tag == "Bomb") {
            Vector3 target = FindObjectOfType<Player>().transform.position;
            target.z = 0f;
            Vector3 objectPos = transform.position;
            target.x = target.x - objectPos.x;
            target.y = target.y - objectPos.y;
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            angle += 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void Update() {
        transform.Rotate(0, 0, speedOfSpin * Time.deltaTime);
    }
}
