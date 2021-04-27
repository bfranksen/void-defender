using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [Header("Values")]
    [SerializeField] public static int laserDamage = 2;
    [SerializeField] public static int pointMod = 2;
    [SerializeField] public static int zapDamage = 150;

    [Header("Probabilities")]
    [SerializeField] static float puDamageOdds = 0.055f; // 55/1000 .055
    [SerializeField] static float puLifeOdds = 0.056f; // 1/1000 .001
    [SerializeField] static float puPointsOdds = 0.1f; // 44/1000 .044
    [SerializeField] static float puRepairOdds = 0.11f; // 10/1000 .01
    [SerializeField] static float puShieldOdds = 0.125f; // 15/1000 .015
    [SerializeField] static float puZapOdds = 0.16f; // 35/1000 .035

    [Header("SFX")]
    [SerializeField] AudioClip availableSFX;
    [SerializeField] [Range(0, 1)] float availableSFXVolume = 0.1f;
    [SerializeField] AudioClip earnedSFX;
    [SerializeField] [Range(0, 1)] float earnedSFXVolume = 0.1f;

    MusicPlayer musicPlayer;

    // Start is called before the first frame update
    private void Start() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        musicPlayer.PlayOneShot(availableSFX, availableSFXVolume);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
        if (transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0, 0)).x) {
            transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);
        } else if (transform.position.x > Camera.main.ViewportToWorldPoint(new Vector3(0.95f, 0, 0)).x) {
            transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
        }
    }

    public void Hit() {
        musicPlayer.PlayOneShot(earnedSFX, earnedSFXVolume);
        Destroy(gameObject, 0.1f);
    }

    public static int GetPowerUpIndex(float random, bool boss) {
        float bossMod = 1f;
        if (boss) bossMod = 5f;
        if (random < puDamageOdds * bossMod) {
            return 0;
        } else if (random < puLifeOdds * bossMod) {
            return 1;
        } else if (random < puPointsOdds * bossMod) {
            return 2;
        } else if (random < puRepairOdds * bossMod) {
            return 3;
        } else if (random < puShieldOdds * bossMod) {
            return 4;
        } else if (random < puZapOdds * bossMod) {
            return 5;
        } else {
            return -1;
        }
    }

    public bool IsTypeDamage(string tag) {
        return tag == "PU-Damage";
    }
    public bool IsTypeLife(string tag) {
        return tag == "PU-Life";
    }
    public bool IsTypePoints(string tag) {
        return tag == "PU-Points";
    }
    public bool IsTypeRepair(string tag) {
        return tag == "PU-Repair";
    }
    public bool IsTypeShield(string tag) {
        return tag == "PU-Shield";
    }
    public bool IsTypeZap(string tag) {
        return tag == "PU-Zap";
    }
}
