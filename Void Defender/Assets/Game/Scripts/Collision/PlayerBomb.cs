using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour {

    [Header("Explosion w/Collider")]
    [SerializeField] GameObject explosionPrefab;

    [Header("SFX")]
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] [Range(0, 1)] float explosionSFXVolume = 0.5f;
    Vector3 crosshairPos;

    private void Start() {
        crosshairPos = Crosshair.targetedPosition;
        Vector3 target = crosshairPos;
        target.z = 0f;
        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        angle -= 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update() {
        if (Vector3.Distance(transform.position, crosshairPos) < 1) {
            CreateExplosion();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 9) { // Enemy
            CreateExplosion();
        }
    }

    private void CreateExplosion() {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
        FindObjectOfType<MusicPlayer>().PlayOneShot(explosionSFX, explosionSFXVolume);
        Destroy(explosion, 1.0f);
        Destroy(gameObject);
    }
}
