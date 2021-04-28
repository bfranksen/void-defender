using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHighScoreDisplay : MonoBehaviour {

    [SerializeField] List<TextMeshProUGUI> highScoreTexts;

    private void Start() {
        List<int> highScores = PlayerPrefsController.GetHighScores();
        for (int i = 0; i < highScores.Count; i++) {
            highScoreTexts[i].text = highScores[i].ToString();
        }
    }
}
