using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { PlayerLaser, PlayerBomb, SmallLaser, NormalLaser, LargeLaser, Bomb, Jacks, Blast, Fireball };

[CreateAssetMenu(menuName = "Weapon Config")]
public class Weapon : ScriptableObject {

    [Header("Type")]
    [SerializeField] WeaponType weaponType;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxClip;
    [SerializeField] [Range(0, 1)] float sfxVolume = 0.2f;

    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public float MinTimeBetweenShots { get => minTimeBetweenShots; set => minTimeBetweenShots = value; }
    public float MaxTimeBetweenShots { get => maxTimeBetweenShots; set => maxTimeBetweenShots = value; }
    public AudioClip SfxClip { get => sfxClip; set => sfxClip = value; }
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }
}
