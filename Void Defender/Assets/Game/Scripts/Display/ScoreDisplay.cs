using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

    Text scoreText;
    GameSession gameSession;

    // Start is called before the first frame update
    private void Start() {
        scoreText = GetComponent<Text>();
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    private void Update() {
        if (!gameSession) {
            gameSession = FindObjectOfType<GameSession>();
        }
        scoreText.text = gameSession.Score.ToString();
    }
}
