using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    public static Vector3 targetedPosition;

    private void Start() {
        targetedPosition = Vector3.zero;
    }

    private void OnDestroy() {
        targetedPosition = transform.position;
    }
}
