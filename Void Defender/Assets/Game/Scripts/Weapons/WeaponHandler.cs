using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponHandler : MonoBehaviour {

    // Global
    List<Weapon> weapons;
    List<float> shotCounter;
    List<bool> shotReady;
    List<Coroutine> firingCoroutines;
    MusicPlayer musicPlayer;

    // Enemy
    Enemy enemy;
    float xMin;
    float xMax;

    // Player
    Player player;
    Movement movement;
    bool isMobile = false;
    bool firingInput = false;
    bool bombBeingAimed = false;
    bool bombFired = false;
    Coroutine aimingCoroutine;
    [SerializeField] GameObject crosshairPrefab;
    GameObject crosshair;
    bool puDamage;
    BossGunPos bossGunPos;
    GameObject projectileParent;
    private const string PROJECTILE_PARENT_NAME = "Projectiles";

    private void Start() {
        CreateProjectileParent();
        SetUpFields();
        for (int index = 0; index < weapons.Count; index++) {
            SetUpCoroutines(index);
            HandleShotCounter(index, true);
        }
    }

    private void Update() {
        for (int index = 0; index < weapons.Count; index++) {
            HandleShotCounter(index, false);
        }
        if (!enemy) {
            GetInput();
        }
    }

    private void FixedUpdate() {
        for (int index = 0; index < weapons.Count; index++) {
            if (!enemy) {
                PlayerWeapons(index);
            } else {
                EnemyWeapons(index);
            }
        }
    }

    private void CreateProjectileParent() {
        projectileParent = GameObject.Find(PROJECTILE_PARENT_NAME);
        if (!projectileParent) {
            projectileParent = new GameObject(PROJECTILE_PARENT_NAME);
        }
    }

    private void SetUpFields() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        if (player) {
            weapons = player.Weapons;
            movement = player.GetComponent<Movement>();
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
            firingInput = true;
#endif
        } else {
            weapons = enemy.Weapons;
            xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.025f, 0, 0)).x;
            xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.975f, 0, 0)).x;
            player = FindObjectOfType<Player>();
            bossGunPos = GetComponent<BossGunPos>();
        }
        shotCounter = new List<float>();
        shotReady = new List<bool>();
        firingCoroutines = new List<Coroutine>();
        puDamage = false;
    }

    private void SetUpCoroutines(int index) {
        firingCoroutines.Add(null);
    }

    private void HandleShotCounter(int index, bool initial) {
        if (initial) {
            shotCounter.Add(Random.Range(weapons[index].MinTimeBetweenShots, weapons[index].MaxTimeBetweenShots));
            shotReady.Add(true);
        } else {
            if (shotCounter[index] > 0) {
                shotCounter[index] -= Time.deltaTime;
            }
        }
    }

    private void GetInput() {
        puDamage = player.PuDamage;
        if (!isMobile) {
            if (!firingInput && Input.GetAxis("Fire1") > 0) {
                firingInput = true;
            }
            if (firingInput && Input.GetAxis("Fire1") <= 0) {
                firingInput = false;
            }

            if (shotReady[1] && shotCounter[1] <= 0 && bombBeingAimed && !bombFired && player.CanFire && Input.GetButtonDown("Fire2")) {
                bombFired = true;
                RemoveCrosshair();
            }
            if (shotReady[1] && shotCounter[1] <= 0 && bombBeingAimed && !bombFired) {
                crosshair.transform.position = movement.AimCrosshair(crosshair);
            }
            if (shotReady[1] && shotCounter[1] <= 0 && !bombBeingAimed && !bombFired && player.CanFire && Input.GetButtonDown("Fire2")) {
                ReadyCrosshair();
            }

            if (bombBeingAimed && Input.GetButtonDown("Cancel")) {
                bombFired = false;
                RemoveCrosshair();
            }
        }
    }

    private void ReadyCrosshair() {
        bombBeingAimed = true;
        crosshair = Instantiate(crosshairPrefab, Vector2.zero, Quaternion.identity) as GameObject;
        if (aimingCoroutine == null) {
            aimingCoroutine = StartCoroutine(AimTimer());
        }
    }

    private void RemoveCrosshair() {
        Crosshair.targetedPosition = crosshair.transform.position;
        bombBeingAimed = false;
        Destroy(crosshair);
        if (aimingCoroutine != null) {
            StopCoroutine(aimingCoroutine);
            aimingCoroutine = null;
        }
    }

    private IEnumerator AimTimer() {
        yield return new WaitForSeconds(6f);
        bombBeingAimed = false;
        bombFired = false;
        Destroy(crosshair);
        if (aimingCoroutine != null) {
            aimingCoroutine = null;
        }
    }

    private void PlayerWeapons(int index) {
        if (index == 0) {
            if (firingCoroutines[index] == null && firingInput && player.CanFire) {
                firingCoroutines[index] = StartCoroutine(FireWeapon(index));
            } else if (firingCoroutines[index] != null && (!firingInput || !player.CanFire)) {
                StopCoroutine(firingCoroutines[index]);
                firingCoroutines[index] = null;
            }
        } else if (index == 1) {
            if (firingCoroutines[index] == null && shotReady[1] && bombFired && player.CanFire) {
                firingCoroutines[index] = StartCoroutine(FireWeapon(index));
                shotReady[1] = false;
            }
        }
    }

    private void EnemyWeapons(int index) {
        float currentX = transform.position.x;
        bool withinBounds = currentX > xMin && currentX < xMax;
        bool shotAvailable = shotCounter[index] <= 0 && shotReady[index];
        if (firingCoroutines[index] == null && withinBounds && shotAvailable) {
            firingCoroutines[index] = StartCoroutine(FireWeapon(index));
            shotReady[index] = false;
        }
    }

    private IEnumerator FireWeapon(int index) {
        switch (weapons[index].WeaponType) {
            case WeaponType.PlayerLaser:
                return FirePlayerLaser(weapons[index]);
            case WeaponType.PlayerBomb:
                return FirePlayerBomb(weapons[index]);
            case WeaponType.SmallLaser:
            case WeaponType.NormalLaser:
            case WeaponType.LargeLaser:
            case WeaponType.Bomb:
                return FireLaserOrBomb(index);
            case WeaponType.Jacks:
                return FireJacks(index);
            default:
                return null;
        }
    }

    private IEnumerator FirePlayerLaser(Weapon weapon) {
        while (true) {
            GameObject laser = Instantiate(weapon.ProjectilePrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, weapon.ProjectileSpeed);
            musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
            laser.transform.parent = projectileParent.transform;
            if (puDamage) {
                yield return new WaitForSeconds(weapon.MinTimeBetweenShots / 1.5f);
            } else {
                yield return new WaitForSeconds(weapon.MinTimeBetweenShots);
            }
        }
    }

    private IEnumerator FirePlayerBomb(Weapon weapon) {
        GameObject playerBomb = Instantiate(weapon.ProjectilePrefab, transform.position, Quaternion.identity) as GameObject;
        playerBomb.GetComponent<Rigidbody2D>().velocity = GetBombVelocityVector(weapon, transform.position, Crosshair.targetedPosition);
        musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
        playerBomb.transform.parent = projectileParent.transform;
        shotCounter[1] = weapon.MinTimeBetweenShots;
        yield return new WaitForSeconds(weapon.MinTimeBetweenShots);
        firingCoroutines[1] = null;
        shotReady[1] = true;
        bombFired = false;
    }

    private IEnumerator FireLaserOrBomb(int index) {
        Weapon weapon = weapons[index];
        musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
        CreateLaser(weapon, index);
        shotCounter[index] = GetNewShotCooldown(index);
        shotCounter[index] *= 0.75f;
        yield return new WaitForSeconds(shotCounter[index]);
        firingCoroutines[index] = null;
        shotReady[index] = true;
    }

    private void CreateLaser(Weapon weapon, int index) {
        if (tag == "Jacks") {
            GetJacksLaser(weapon, index);
        } else {
            GetLaser(weapon);
        }
    }

    private void GetJacksLaser(Weapon weapon, int index) {
        GameObject laser = Instantiate(weapon.ProjectilePrefab, bossGunPos.GetGunPosition(index).position, transform.rotation) as GameObject;
        var vector = GetVelocityVector(weapon, true);
        laser.GetComponent<Rigidbody2D>().velocity = vector;
        laser.transform.parent = projectileParent.transform;
    }

    private void GetLaser(Weapon weapon) {
        var laser = Instantiate(weapon.ProjectilePrefab, transform.position, transform.rotation) as GameObject;
        var vector = GetVelocityVector(weapon, false);
        laser.GetComponent<Rigidbody2D>().velocity = vector;
        laser.transform.parent = projectileParent.transform;
    }

    private Vector2 GetVelocityVector(Weapon weapon, bool jacksLaser) {
        if (player && weapon.WeaponType == WeaponType.Bomb) {
            return GetBombVelocityVector(weapon, transform.position, player.transform.position);
        }
        if (jacksLaser) {
            if (IsPlayerBelow()) {
                return new Vector2(0, -weapon.ProjectileSpeed * 1.5f);
            } else {
                return new Vector2(0, weapon.ProjectileSpeed * 1.5f);
            }
        }
        return new Vector2(0, -weapon.ProjectileSpeed);
    }

    private Vector2 GetBombVelocityVector(Weapon weapon, Vector3 sourcePos, Vector3 targetPos) {
        float xDiff = targetPos.x - sourcePos.x;
        float yDiff = targetPos.y - sourcePos.y;
        float distance = Mathf.Sqrt(Mathf.Pow(xDiff, 2f) + Mathf.Pow(yDiff, 2f));
        return new Vector2(xDiff * weapon.ProjectileSpeed / distance, yDiff * weapon.ProjectileSpeed / distance);
    }

    private IEnumerator FireJacks(int index) {
        Weapon weapon = weapons[index];
        musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
        StartCoroutine(PreparingToFireAnimation());
        yield return new WaitForSeconds(0.5f);
        int numProjectiles = Random.Range(12, 19);
        float rotationIncrement = Mathf.PI * 2.0f / numProjectiles;
        for (int i = 0; i < numProjectiles; i++) {
            FireSingleJack(weapon, i, rotationIncrement, 0);
        }
        shotCounter[index] = GetNewShotCooldown(index);
        yield return new WaitForSeconds(shotCounter[index]);
        firingCoroutines[index] = null;
        shotReady[index] = true;
    }

    private void FireSingleJack(Weapon weapon, int index, float rotationIncrement, int rotationOffset) {
        GameObject jack = Instantiate(weapon.ProjectilePrefab, bossGunPos.GetGunPosition(weapons.IndexOf(weapon)).position, transform.rotation) as GameObject;
        float vx = Mathf.Cos(rotationIncrement * index + rotationOffset) * weapon.ProjectileSpeed;
        float vy = Mathf.Sin(rotationIncrement * index + rotationOffset) * weapon.ProjectileSpeed;
        Vector2 vector = new Vector2(vx, vy);
        jack.GetComponent<Rigidbody2D>().velocity = vector;
        jack.transform.parent = projectileParent.transform;
    }

    private IEnumerator PreparingToFireAnimation() {
        musicPlayer.PlayOneShot(enemy.HitSFX, enemy.HitSFXVolume);
        enemy.SpriteRenderer.color = new Color(0.8431373f, 0.5882353f, 0.5882353f);
        yield return new WaitForSeconds(1f);
        enemy.SpriteRenderer.color = Color.white;
    }

    private float GetNewShotCooldown(int index) {
        return Mathf.Max(weapons[index].MinTimeBetweenShots, Random.Range(weapons[index].MinTimeBetweenShots, weapons[index].MaxTimeBetweenShots) - EnemySpawner.gameModifier);
    }

    private bool IsPlayerBelow() {
        if (!player) return 0 < transform.position.y;
        return player.transform.position.y < transform.position.y;
    }
    public float GetPlayerBombCooldown() {
        if (weapons != null) {
            return weapons[1].MinTimeBetweenShots;
        } else {
            return -1;
        }
    }
    public float GetPlayBombCooldownRemaining() {
        return shotCounter[1];
    }
}
