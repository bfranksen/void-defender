using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHighScoreDisplay : MonoBehaviour {

    [Header("High Scores")]
    [SerializeField] List<TextMeshProUGUI> highScoreTexts;
    [SerializeField] TextMeshProUGUI titleText;

    private void Start() {
        Refresh();
    }

    public void Refresh() {
        SetLeaderboardText();
        List<int> highScores = PlayerPrefsController.GetHighScores();
        if (highScores != null) {
            for (int i = 0; i < 10; i++) {
                if (PlayerPrefsController.HasHighScore(i)) {
                    highScoreTexts[i].text = highScores[i].ToString();
                } else {
                    highScoreTexts[i].text = "-----";
                }
            }
        } else {
            for (int i = 0; i < 10; i++) {
                highScoreTexts[i].text = "-----";
            }
            Debug.LogWarning("Must have a profile to track high scores");
        }
    }

    private void SetLeaderboardText() {
        if (PlayerPrefsController.GetUserAccounts().Count > 0) {
            string currentUser = PlayerPrefsController.GetCurrentUserAccount();
            titleText.text = PlayerPrefsController.GetCurrentUserAccount() + "'s High Scores";
        } else {
            titleText.text = "Create or choose a profile to track your high scores.";
        }
    }
}
