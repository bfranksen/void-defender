using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // ---------------------------------------------------------------------------------------------------------------------------
    //  FIELDS
    // ---------------------------------------------------------------------------------------------------------------------------

    [Header("Player")]
    [SerializeField] int startingHealth = 200;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float xPadding = 0.5f;
    [SerializeField] float yPadding = 0.5f;

    [Header("Weapons")]
    [SerializeField] List<Weapon> weapons;

    [Header("Power Ups")]
    [SerializeField] float puTimeEmpowered = 6f;

    [Header("Shields")]
    [SerializeField] GameObject respawnShieldPrefab;
    [SerializeField] GameObject puShieldPrefab;

    [Header("VFX")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] GameObject zapVFX;

    [Header("SFX")]
    [SerializeField] AudioClip hitSFX;
    [SerializeField] [Range(0, 1)] float hitSFXVolume = 0.5f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume = 0.5f;

    // Respawn
    Vector3 respawnPos;
    bool respawning = false;
    bool canFire = true;

    // Power Ups
    bool puDamage = false;
    bool puPoints = false;
    bool puShield = false;
    bool puZap = false;
    float puDamageTimeLeft = 0;
    float puPointsTimeLeft = 0;

    // Coroutines
    Coroutine damageModCoroutine;
    Coroutine pointModCoroutine;

    // General
    MusicPlayer musicPlayer;
    GameSession gameSession;
    float deltaTime;

    // Player movement
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Touch movement
#if UNITY_ANDROID || UNITY_IOS
    private float width;
    private float height;
#endif

    // Get: Set:
    public bool PuDamage { get => puDamage; set => puDamage = value; }
    public bool PuPoints { get => puPoints; set => puPoints = value; }
    public bool PuShield { get => puShield; set => puShield = value; }
    public bool PuZap { get => puZap; set => puZap = value; }
    public float PuDamageTimeLeft { get => puDamageTimeLeft; set => puDamageTimeLeft = value; }
    public float PuPointsTimeLeft { get => puPointsTimeLeft; set => puPointsTimeLeft = value; }
    public List<Weapon> Weapons { get => weapons; set => weapons = value; }
    public bool CanFire { get => canFire; set => canFire = value; }



    // ---------------------------------------------------------------------------------------------------------------------------
    //  SCRIPT
    // ---------------------------------------------------------------------------------------------------------------------------

    private void Start() {
        Debug.Log("Screen Res: " + Screen.currentResolution + "  -  Screen Aspect Ratio: " + Screen.width * 1.0f / Screen.height);
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
        SetupFields();
        SetUpMoveBoundaries();
        StartCoroutine(GetFps()); // dev mode only
    }

    private void Update() {
        deltaTime = Time.deltaTime;
        Move();
        PowerUpTimers();
        if (dm_FpsCounter) {
            updateCount++;
        }
    }

    private void SetupFields() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        if (!gameSession) {
            gameSession = FindObjectOfType<GameSession>();
        }
    }

    private void SetUpMoveBoundaries() {
        Camera gameCamera = Camera.main;
        Renderer renderer = GetComponent<Renderer>();

        Vector3 bottomeLeftWorldCoordinates = gameCamera.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRightWorldCoordinates = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Vector3 movementRangeMin = bottomeLeftWorldCoordinates + renderer.bounds.extents;
        Vector3 movementRangeMax = topRightWorldCoordinates - renderer.bounds.extents;

        respawnPos = gameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, 0));
        xMin = movementRangeMin.x + xPadding;
        xMax = movementRangeMax.x - xPadding;
        yMin = movementRangeMin.y + yPadding;
        yMax = movementRangeMax.y - yPadding * 4;
    }

    private void Move() {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            Debug.Log("Touch: " + touch);
            if (touch.phase == TouchPhase.Moved) {
                Vector2 pos = touch.position;
                Debug.Log("Touch Pos: " + pos);
                // pos.x = (pos.x - width) / width;
                // pos.y = (pos.y - height) / height;
                // transform.position = new Vector3(-pos.x, pos.y, 0.0f);
            }
        }
#else
        var deltaX = Input.GetAxis("Horizontal") * moveSpeed * deltaTime;
        var deltaY = Input.GetAxis("Vertical") * moveSpeed * deltaTime;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
#endif
    }

    private void PowerUpTimers() {
        if (puDamageTimeLeft > 0) {
            puDamageTimeLeft -= deltaTime;
        }
        if (puPointsTimeLeft > 0) {
            puPointsTimeLeft -= deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (!respawning && !dm_Invulnerable) {
            DamageDealer damageDealer = collider.gameObject.GetComponent<DamageDealer>();
            if (damageDealer) {
                ProcessHit(damageDealer);
            }
        }
        PowerUp powerUp = collider.gameObject.GetComponent<PowerUp>();
        if (powerUp) {
            ProcessPowerUp(powerUp);
        }
    }

    private void ProcessHit(DamageDealer damageDealer) {
        gameSession.Health -= damageDealer.BaseDamage;
        if (gameSession.Health <= 0) {
            ProcessDeath();
        } else {
            StartCoroutine(ProjectileHit(0.4f));
        }
        if (damageDealer.gameObject.layer != 13) {
            damageDealer.Hit();
        }
    }

    private IEnumerator ProjectileHit(float animationLength) {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        musicPlayer.PlayOneShot(hitSFX, hitSFXVolume);
        spriteRenderer.color = new Color(0.8431373f, 0.5882353f, 0.5882353f);
        yield return new WaitForSeconds(animationLength);
        spriteRenderer.color = Color.white;
    }

    public void ProcessDeath() {
        respawning = true;
        CanFire = false;
        gameSession.Lives -= 1;
        if (gameSession.Lives > 0) {
            StartCoroutine(Respawn());
        } else {
            GameOver();
        }
    }

    private IEnumerator Respawn() {
        // DestroyProjectiles(); // Not sure if this will be used here, but keeping code for potential use elsewhere
        DestroyPowerUps();
        GetComponent<Renderer>().enabled = false;
        yield return StartCoroutine(DeathAnimation());
        transform.position = respawnPos;
        GetComponent<Renderer>().enabled = true;
        GainShield(respawnShieldPrefab, false);
        gameSession.Health = startingHealth;
        respawning = false;
    }


    private void GameOver() {
        StartCoroutine(DeathAnimation());
        Destroy(gameObject);
        FindObjectOfType<Level>().LoadGameOver();
    }

    private IEnumerator DeathAnimation() {
        musicPlayer.PlayOneShot(deathSFX, deathSFXVolume);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation) as GameObject;
        Destroy(explosion, 1f);
        yield return new WaitForSeconds(0.75f);
    }

    private void DestroyProjectiles() {
        GameObject[] gos = FindObjectsOfType<GameObject>() as GameObject[];
        foreach (GameObject go in gos) {
            if (go.layer == 10 || go.layer == 11) {
                Destroy(go);
            }
        }
    }

    private void DestroyPowerUps() {
        GameObject[] gos = FindObjectsOfType<GameObject>() as GameObject[];
        foreach (GameObject go in gos) {
            if (go.layer == 12) {
                Destroy(go);
            }
        }
        if (damageModCoroutine != null) {
            StopCoroutine(damageModCoroutine);
            PUDamageEffect(false);
        }
        if (pointModCoroutine != null) {
            StopCoroutine(pointModCoroutine);
            PUPointsEffect(false);
        }
    }

    private void ProcessPowerUp(PowerUp powerUp) {
        powerUp.Hit();
        if (powerUp.IsTypeDamage(powerUp.tag)) {
            if (damageModCoroutine != null) {
                StopCoroutine(damageModCoroutine);
            }
            damageModCoroutine = StartCoroutine(PUDamage());
        } else if (powerUp.IsTypeLife(powerUp.tag)) {
            gameSession.Lives++;
        } else if (powerUp.IsTypePoints(powerUp.tag)) {
            if (pointModCoroutine != null) {
                StopCoroutine(pointModCoroutine);
            }
            pointModCoroutine = StartCoroutine(PUPoints());
        } else if (powerUp.IsTypeRepair(powerUp.tag)) {
            gameSession.Health = startingHealth;
        } else if (powerUp.IsTypeShield(powerUp.tag)) {
            if (!puShield) GainShield(puShieldPrefab, true);
        } else if (powerUp.IsTypeZap(powerUp.tag)) {
            StartCoroutine(ZapEnemies());
        }
    }

    private IEnumerator PUDamage() {
        PUDamageEffect(true);
        yield return new WaitForSeconds(puTimeEmpowered);
        PUDamageEffect(false);
    }

    private void PUDamageEffect(bool on) {
        if (on) {
            PuDamage = true;
            puDamageTimeLeft = puTimeEmpowered;
        } else {
            PuDamage = false;
            puDamageTimeLeft = 0;
        }
    }

    private IEnumerator PUPoints() {
        PUPointsEffect(true);
        yield return new WaitForSeconds(puTimeEmpowered);
        PUPointsEffect(false);
    }

    private void PUPointsEffect(bool on) {
        if (on) {
            PuPoints = true;
            puPointsTimeLeft = puTimeEmpowered;
        } else {
            PuPoints = false;
            puPointsTimeLeft = 0;
        }
    }

    private void GainShield(GameObject shieldPrefab, bool isPowerUp) {
        Instantiate(shieldPrefab, transform.position, transform.rotation);
        PuShield = isPowerUp;
        CanFire = true;
    }

    private IEnumerator ZapEnemies() {
        puZap = true;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in enemies) {
            Zap(e);
        }
        yield return new WaitForSeconds(1f);
        puZap = false;
    }

    private void Zap(Enemy enemy) {
        GameObject zap = Instantiate(zapVFX, enemy.transform.position, enemy.transform.rotation) as GameObject;
        zap.GetComponent<Zap>().Enemy = enemy;
        if (enemy.tag == "Spawned") {
            zap.transform.localScale = new Vector2(zap.transform.localScale.x / 2f, zap.transform.localScale.y / 2f);
        }
        Destroy(zap, 1f);
    }



    // ---------------------------------------------------------------------------------------------------------------------------
    // DEV MODE
    // ---------------------------------------------------------------------------------------------------------------------------

    [Header("Dev Mode")]
    [SerializeField] bool dm_FpsCounter = false;
    [SerializeField] bool dm_Invulnerable = false;

    float updateCount = 0;
    float updateCPS = 0;

    private IEnumerator GetFps() {
        if (dm_FpsCounter) {
            while (true) {
                yield return new WaitForSeconds(1);
                updateCPS = updateCount;
                updateCount = 0;
            }
        }
    }

    private void OnGUI() {
        if (dm_FpsCounter) {
            GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
            fontSize.fontSize = 24;
            Vector3 vector = Camera.main.ViewportToScreenPoint(new Vector3(0, 1, 0));
            GUI.Label(new Rect(vector.x + 16, vector.y - 48, 128, 64), "FPS: " + updateCPS.ToString(), fontSize);
        }
    }
}
