using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitParent : MonoBehaviour {

    Transform target;
    float orbitDegreesPerSec = 56.0f;
    Vector3 relativeDistance = Vector3.zero;

    private void Start() {
        target = GameObject.FindGameObjectWithTag("Fireball").transform;
        relativeDistance = transform.position - target.position;
    }

    private void LateUpdate() {
        Orbit();
    }

    private void Orbit() {
        if (target != null) {
            transform.position = target.position + relativeDistance;
            transform.RotateAround(target.position, Vector3.forward, orbitDegreesPerSec * Time.deltaTime);
            relativeDistance = transform.position - target.position;
            if (Time.timeScale > 0) {
                relativeDistance += relativeDistance * 0.001f;
            }
        } else {
            Destroy(gameObject);
        }
    }
}
