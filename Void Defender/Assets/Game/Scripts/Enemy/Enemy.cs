using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Enemy")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Weapon")]
    [SerializeField] List<Weapon> weapons;

    [Header("Power Ups")]
    [SerializeField] GameObject[] powerUpPrefabs;

    [Header("VFX")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] GameObject spawnPrefab;

    [Header("SFX")]
    [SerializeField] AudioClip hitSFX;
    [SerializeField] [Range(0, 1)] float hitSFXVolume = 0.5f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume = 0.4f;

    MusicPlayer musicPlayer;
    SpriteRenderer spriteRenderer;
    WeaponHandler weaponHandler;
    Player player;
    bool hitCoRunning = false;

    public float Health { get => health; set => health = value; }
    public List<Weapon> Weapons { get => weapons; set => weapons = value; }
    public AudioClip HitSFX { get => hitSFX; set => hitSFX = value; }
    public float HitSFXVolume { get => hitSFXVolume; set => hitSFXVolume = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    // Start is called before the first frame update
    private void Start() {
        SetUpFields();
    }

    private void SetUpFields() {
        Health += Health * EnemySpawner.gameModifier;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        weaponHandler = GetComponent<WeaponHandler>();
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        DamageDealer damageDealer = collider.gameObject.GetComponent<DamageDealer>();
        if (damageDealer) {
            ProcessHit(damageDealer);
        }
    }

    private void ProcessHit(DamageDealer damageDealer) {
        musicPlayer.PlayOneShot(HitSFX, HitSFXVolume);
        float dmgMod = EnemySpawner.gameModifier;
        if (player.PuDamage) {
            float boostedDmg = damageDealer.BaseDamage * PowerUp.laserDamage;
            Health -= boostedDmg + boostedDmg * dmgMod;
        } else {
            Health -= damageDealer.BaseDamage + damageDealer.BaseDamage * dmgMod;
        }
        if (Health <= 0) {
            ProcessDeath();
        } else {
            if (!hitCoRunning) {
                StartCoroutine(ProjectileHit(0.12f));
            }
        }
        damageDealer.Hit();
    }

    private IEnumerator ProjectileHit(float runTime) {
        hitCoRunning = true;
        SpriteRenderer.color = new Color(0.8431373f, 0.5882353f, 0.5882353f);
        yield return new WaitForSeconds(runTime);
        SpriteRenderer.color = Color.white;
        hitCoRunning = false;
    }

    private void ProcessDeath() {
        int baseScore = scoreValue * EnemySpawner.pointMultiplier;
        if (player.PuPoints) {
            FindObjectOfType<GameSession>().Score += baseScore * PowerUp.pointMod;
        } else {
            FindObjectOfType<GameSession>().Score += baseScore;
        }
        if (tag == "Spawn") SpawnChildren();
        if (tag != "Spawned") CheckForPowerUp(0);
        if (gameObject.layer == 13 && transform.position.x < 0) {
            CheckForPowerUp(Random.Range(0.5f, 1.5f));
        } else if (gameObject.layer == 13 && transform.position.x >= 0) {
            CheckForPowerUp(-Random.Range(0.5f, 1.5f));
        }
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation) as GameObject;
        Destroy(explosion, 1f);
        musicPlayer.PlayOneShot(deathSFX, deathSFXVolume);
    }

    private void SpawnChildren() {
        Vector2[] spawnPos = { new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -0.5f) };
        for (int i = 0; i < 3; i++) {
            float xPos = transform.position.x + spawnPos[i].x;
            float yPos = transform.position.y + spawnPos[i].y;
            Vector2 vector = new Vector2(xPos, yPos);
            GameObject spawn = Instantiate(spawnPrefab, vector, transform.rotation) as GameObject;
        }
    }

    private void CheckForPowerUp(float pos) {
        Vector3 offset = new Vector3(pos, pos, 0);
        int puIndex = PowerUp.GetPowerUpIndex(Random.value, gameObject.layer == 13);
        if (puIndex > -1) {
            GameObject powerUp = Instantiate(powerUpPrefabs[puIndex], transform.position + offset, transform.rotation) as GameObject;
        }
    }

    public void DoDamage(float damage) {
        Health -= damage;
        if (Health <= 0) {
            ProcessDeath();
        } else {
            if (hitCoRunning) {
                StartCoroutine(ProjectileHit(0.12f));
            }
        }
    }
}
