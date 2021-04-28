using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldType { PowerUp, Respawn };

public class Shield : MonoBehaviour {

    // Serialized Fields
    [Header("Type")]
    [SerializeField] ShieldType shieldType;

    [Header("Respawn")]
    [SerializeField] float respawnProtectionTime;

    [Header("SFX")]
    [SerializeField] AudioClip shieldUpSFX;
    [SerializeField] [Range(0, 1)] float shieldUpSFXVolume = 0.5f;
    [SerializeField] AudioClip shieldDownSFX;
    [SerializeField] [Range(0, 1)] float shieldDownSFXVolume = 0.5f;
    [SerializeField] AudioClip respawnHitSFX;
    [SerializeField] [Range(0, 1)] float respawnHitSFXVolume = 0.5f;

    // Non-Serialized Fields
    MusicPlayer musicPlayer;
    Player player;
    float protectionTimeLeft;

    // Start is called before the first frame update
    private void Start() {
        SetUpShield();
    }

    // Update is called once per frame
    private void Update() {
        if (player) {
            transform.position = player.transform.position;
            transform.position += new Vector3(0, 0.1f, 0);
        }
        if (protectionTimeLeft > 0) {
            protectionTimeLeft -= Time.deltaTime;
        }
    }

    private void SetUpShield() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        player = FindObjectOfType<Player>();
        transform.position = player.transform.position;
        musicPlayer.PlayOneShot(shieldUpSFX, shieldUpSFXVolume);
        if (shieldType == ShieldType.Respawn) {
            StartCoroutine(InitRespawnShield());
        }
    }

    private IEnumerator InitRespawnShield() {
        protectionTimeLeft = respawnProtectionTime;
        yield return new WaitForSeconds(respawnProtectionTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject == player || collider.gameObject.GetComponent<PowerUp>()) {
            return;
        }
        if (shieldType == ShieldType.Respawn) {
            RespawnShieldCollision(collider);
        } else {
            PowerUpShieldCollision(collider);
        }
    }

    private void RespawnShieldCollision(Collider2D collider) {
        if (collider.gameObject.layer == 13 || collider.gameObject.layer == 15) {
            DestroyShield();
        } else {
            musicPlayer.PlayOneShot(respawnHitSFX, respawnHitSFXVolume);
            Destroy(collider.gameObject);
        }
    }

    private void PowerUpShieldCollision(Collider2D collider) {
        DestroyShield();
        player.PuShield = false;
        if (collider.gameObject.layer != 15 || (collider.gameObject.layer != 13 && collider.gameObject.tag != "Jacks")) {
            Destroy(collider.gameObject);
        }
    }

    private void DestroyShield() {
        musicPlayer.PlayOneShot(shieldDownSFX, shieldDownSFXVolume);
        Destroy(gameObject);
    }
}
