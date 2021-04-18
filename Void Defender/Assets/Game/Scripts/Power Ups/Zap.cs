using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : MonoBehaviour {

    [Header("SFX")]
    [SerializeField] AudioClip zapSFX;
    [SerializeField] [Range(0, 1)] float zapSFXVolume = 0.05f;

    MusicPlayer musicPlayer;
    Enemy enemy;
    float xShift;
    float yShift;

    // Start is called before the first frame update
    private void Start() {
        if (enemy) {
            SetUpZap();
        }
    }

    // Update is called once per frame
    private void Update() {
        if (enemy) {
            Vector3 zapPos = new Vector3(enemy.transform.position.x, enemy.transform.position.y + yShift, 0.1f);
            transform.position = zapPos;
        } else {
            Destroy(gameObject, 0.3f);
        }
    }

    private void SetUpZap() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        float height = enemy.GetComponent<Renderer>().bounds.extents.y;
        if (height < 0.5f) {
            yShift = height * 1.75f;
        } else {
            yShift = height;
        }
        transform.Rotate(0, 0, 90);
        enemy.DoDamage(PowerUp.zapDamage + (float)PowerUp.zapDamage * EnemySpawner.gameModifier);
        musicPlayer.PlayOneShot(zapSFX, zapSFXVolume);
    }

    public Enemy Enemy { get => enemy; set => enemy = value; }
}
