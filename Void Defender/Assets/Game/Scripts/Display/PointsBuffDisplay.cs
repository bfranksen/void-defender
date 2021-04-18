using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsBuffDisplay : MonoBehaviour {

    TextMeshProUGUI pointsText;
    Player player;

    // Start is called before the first frame update
    private void Start() {
        pointsText = GetComponent<TextMeshProUGUI>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    private void Update() {
        if (player) {
            pointsText.text = Mathf.CeilToInt(player.PuPointsTimeLeft).ToString();
        }
    }
}
