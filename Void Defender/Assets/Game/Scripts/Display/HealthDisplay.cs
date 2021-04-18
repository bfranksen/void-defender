using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {

    Text healthText;
    GameSession gameSession;

    // Start is called before the first frame update
    private void Start() {
        healthText = GetComponent<Text>();
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    private void Update() {
        if (!gameSession) {
            gameSession = FindObjectOfType<GameSession>();
        }
        if (gameSession.Health < 0) {
            gameSession.Health = 0;
        }
        healthText.text = gameSession.Health.ToString();
    }
}
