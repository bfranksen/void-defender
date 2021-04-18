using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

    Vector3 velocity = Vector3.zero;
    float breakpointHeight;
    float sleepThreshold = 1f;
    float gravity;

    private void Start() {
        if (tag == "BounceUp") {
            gravity = 28f;
            breakpointHeight = transform.position.y + 8f;
        } else {
            gravity = -28f;
            breakpointHeight = transform.position.y - 10f;
        }
    }

    private void FixedUpdate() {
        if (tag == "BounceUp") {
            Up();
        } else {
            Down();
        }
    }

    private void Down() {
        if (velocity.magnitude > sleepThreshold || transform.position.y > breakpointHeight) {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        transform.position += velocity * Time.fixedDeltaTime;
        if (transform.position.y <= breakpointHeight) {
            transform.position = new Vector2(transform.position.x, breakpointHeight);
            velocity.y = -velocity.y;
        }
    }

    private void Up() {
        if (velocity.magnitude > sleepThreshold || transform.position.y < breakpointHeight) {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        transform.position += velocity * Time.fixedDeltaTime;
        if (transform.position.y >= breakpointHeight) {
            transform.position = new Vector2(transform.position.x, breakpointHeight);
            velocity.y = -velocity.y;
        }
    }
}

