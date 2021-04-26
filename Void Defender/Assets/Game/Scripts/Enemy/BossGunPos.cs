using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGunPos : MonoBehaviour {

    [SerializeField] GameObject[] guns;

    public Transform GetGunPosition(int gunIndex) {
        return guns[gunIndex].transform;
    }
}
