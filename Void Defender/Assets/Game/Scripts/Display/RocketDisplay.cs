using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketDisplay : MonoBehaviour {

    private Image rocketImage;
    private WeaponHandler weaponHandler;
    private float cooldown;

    private void Start() {
        rocketImage = GetComponent<Image>();
        weaponHandler = FindObjectOfType<Player>().GetComponent<WeaponHandler>();
        if (weaponHandler) {
            cooldown = weaponHandler.GetPlayerBombCooldown();
        }
    }

    private void Update() {
        if (weaponHandler && cooldown <= 0) {
            cooldown = weaponHandler.GetPlayerBombCooldown();
        }
        rocketImage.fillAmount = 1 - (weaponHandler.GetPlayBombCooldownRemaining() / cooldown);
    }
}
