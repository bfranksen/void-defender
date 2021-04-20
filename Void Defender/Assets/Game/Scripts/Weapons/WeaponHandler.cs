using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool isMobile = false;
    bool firingInput = false;
    bool puDamage;

    private void Start() {
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

    private void SetUpFields() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        if (player) {
            weapons = player.Weapons;
#if UNITY_ANDROID || UNITY_IOS
            // Debug.Log("Mobile: Auto-firing enabled");
            isMobile = true;
            firingInput = true;
#endif
        } else {
            weapons = enemy.Weapons;
            xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.025f, 0, 0)).x;
            xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.975f, 0, 0)).x;
            player = FindObjectOfType<Player>();
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
            if (!firingInput && Input.GetButtonDown("Fire1")) {
                firingInput = true;
            }
            if (firingInput && Input.GetButtonUp("Fire1")) {
                firingInput = false;
            }
        }
    }

    private void PlayerWeapons(int index) {
        if (firingCoroutines[index] == null && firingInput && player.CanFire) {
            firingCoroutines[index] = StartCoroutine(FireWeapon(index));
        } else if (firingCoroutines[index] != null && (!firingInput || !player.CanFire)) {
            StopCoroutine(firingCoroutines[index]);
            firingCoroutines[index] = null;
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
            if (puDamage) {
                yield return new WaitForSeconds(weapon.MinTimeBetweenShots / 1.5f);
            } else {
                yield return new WaitForSeconds(weapon.MinTimeBetweenShots);
            }
        }
    }

    private IEnumerator FireLaserOrBomb(int index) {
        Weapon weapon = weapons[index];
        musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
        CreateLaser(weapon, index);
        shotCounter[index] = GetNewShotCooldown(index);
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
        GameObject laser;
        Vector3 offset;
        Renderer renderer = GetComponent<Renderer>();
        offset = new Vector3(renderer.bounds.extents.x / 3, renderer.bounds.extents.y, 0);
        offset.y *= IsPlayerBelow() ? -1 : 1;
        if (index == 1) {
            offset.x *= -1;
            laser = Instantiate(weapon.ProjectilePrefab, transform.position + offset, transform.rotation) as GameObject;
        } else {
            laser = Instantiate(weapon.ProjectilePrefab, transform.position + offset, transform.rotation) as GameObject;
        }
        var vector = GetVelocityVector(weapon, true);
        laser.GetComponent<Rigidbody2D>().velocity = vector;
    }

    private void GetLaser(Weapon weapon) {
        var laser = Instantiate(weapon.ProjectilePrefab, transform.position, transform.rotation) as GameObject;
        var vector = GetVelocityVector(weapon, false);
        laser.GetComponent<Rigidbody2D>().velocity = vector;
    }

    private Vector2 GetVelocityVector(Weapon weapon, bool jacksLaser) {
        if (player && weapon.WeaponType == WeaponType.Bomb) {
            return GetBombVelocityVector(weapon);
        }
        if (jacksLaser) {
            if (IsPlayerBelow()) {
                return new Vector2(0, -weapon.ProjectileSpeed * 2.25f);
            } else {
                return new Vector2(0, weapon.ProjectileSpeed * 2.25f);
            }
        }
        return new Vector2(0, -weapon.ProjectileSpeed);
    }

    private Vector2 GetBombVelocityVector(Weapon weapon) {
        float xDiff = player.transform.position.x - transform.position.x;
        float yDiff = player.transform.position.y - transform.position.y;
        float distance = Mathf.Sqrt(Mathf.Pow(xDiff, 2f) + Mathf.Pow(yDiff, 2f));
        return new Vector2(xDiff * weapon.ProjectileSpeed / distance, yDiff * weapon.ProjectileSpeed / distance);
    }

    private IEnumerator FireJacks(int index) {
        Weapon weapon = weapons[index];
        musicPlayer.PlayOneShot(weapon.SfxClip, weapon.SfxVolume);
        StartCoroutine(PreparingToFireAnimation());
        yield return new WaitForSeconds(0.5f);
        int numProjectiles = 18;
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
        GameObject jack = Instantiate(weapon.ProjectilePrefab, transform.position, transform.rotation) as GameObject;
        float vx = Mathf.Cos(rotationIncrement * index + rotationOffset) * weapon.ProjectileSpeed;
        float vy = Mathf.Sin(rotationIncrement * index + rotationOffset) * weapon.ProjectileSpeed;
        Vector2 vector = new Vector2(vx, vy);
        jack.GetComponent<Rigidbody2D>().velocity = vector;
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
}
