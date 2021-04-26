using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour {

    [SerializeField] int baseDamage = 100;

    public void Hit() {
        if (!GetComponent<BombExplosion>()) {
            Destroy(gameObject);
        }
    }

    public int BaseDamage { get => baseDamage; }
}
