using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LivesDisplay : MonoBehaviour {

    TextMeshProUGUI livesText;
    GameSession gameSession;

    // Start is called before the first frame update
    private void Start() {
        livesText = GetComponent<TextMeshProUGUI>();
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    private void Update() {
        if (!gameSession) {
            gameSession = FindObjectOfType<GameSession>();
        }
        livesText.text = gameSession.Lives.ToString();
    }
}
