using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

    [Header("VFX")]
    [SerializeField] GameObject deathVFX;

    [Header("SFX")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (gameObject.layer == 11 && collider.gameObject.layer == 10 && collider.gameObject.tag != "Bomb") { // Player Projectile
            GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation) as GameObject;
            Destroy(explosion, 1f);
            FindObjectOfType<MusicPlayer>().PlayOneShot(deathSFX, deathSFXVolume);
            Destroy(gameObject);
            Destroy(collider.gameObject);
        } else if (gameObject.layer == 15 && (collider.gameObject.layer == 10 || collider.gameObject.layer == 11)) {
            GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation) as GameObject;
            Destroy(explosion, 1f);
            FindObjectOfType<MusicPlayer>().PlayOneShot(deathSFX, deathSFXVolume);
            Destroy(collider.gameObject);
        }
    }
}
