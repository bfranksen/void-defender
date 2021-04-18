using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageBuffDisplay : MonoBehaviour {

    TextMeshProUGUI damageText;
    Player player;

    // Start is called before the first frame update
    private void Start() {
        damageText = GetComponent<TextMeshProUGUI>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    private void Update() {
        if (player) {
            damageText.text = Mathf.CeilToInt(player.PuDamageTimeLeft).ToString();
        }
    }
}
