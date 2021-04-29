﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameOverMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject mainMenuButton;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;
    float buttonScale = 0.8f;

    // Start is called before the first frame update
    private void Start() {
        int score = FindObjectOfType<GameSession>().Score;
        PlayerPrefsController.AttemptToAddHighScore(score);
        string username = PlayerPrefsController.GetCurrentUserAccount();
        if (username != "") {
            FindObjectOfType<HighScores>().AddNewHighscore(username, score);
        }
        SetInitialObject();
        SetSizeDeltas();
#if UNITY_ANDROID || UNITY_IOS
        ChangeButtonColorScheme();
#endif
    }

    // Update is called once per frame
    private void Update() {
        ResetCurrentSelected();
    }

    private void SetInitialObject() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        GameObject go = EventSystem.current.currentSelectedGameObject;
        go.GetComponent<Animator>().enabled = true;
        TextMeshPro tmp = go.GetComponent<TextMeshPro>();
        if (tmp) {
            tmp.color = new Color32(255, 143, 0, 255);
        }
    }

    private void SetSizeDeltas() {
        Vector2 rtSizeDelta = new Vector2(Screen.width / -16f, 0);
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        for (int i = 0; i < rt.transform.childCount; i++) {
            if (i == 1) {
                rtSizeDelta *= 2;
            }
            GameObject child = rt.transform.GetChild(i).gameObject;
            RectTransform childRt = child.GetComponent<RectTransform>();
            childRt.sizeDelta += rtSizeDelta;
        }
    }
    private void ChangeButtonColorScheme() {
        foreach (Button button in FindObjectsOfType<Button>()) {
            if (button != firstButton.GetComponent<Button>()) {
                ColorBlock colorBlock = button.colors;
                colorBlock.highlightedColor = Color.white;
                colorBlock.selectedColor = Color.white;
                button.colors = colorBlock;
            }
        }
    }

    private void ResetCurrentSelected() {
        if (EventSystem.current.currentSelectedGameObject != recentSelectedObject) {
            lastSelectedObject = recentSelectedObject;
            recentSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
        if (!EventSystem.current.currentSelectedGameObject) {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        }
#if !UNITY_ANDROID && !UNITY_IOS
        EnableAnimator();
#endif
    }

    private void EnableAnimator() {
        GameObject lastObj = lastSelectedObject;
        if (lastObj && lastObj.GetComponent<Animator>()) {
            lastObj.GetComponent<Animator>().enabled = false;
            lastObj.transform.localScale = new Vector3(buttonScale, buttonScale, 0);
        }
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj && currentObj.GetComponent<Animator>()) {
            currentObj.GetComponent<Animator>().enabled = true;
        }
    }
}
