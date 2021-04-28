using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderboardMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;
    Color32 appOrange = new Color32(255, 143, 0, 255);

    private void Start() {
        SetInitialObject();
    }

    private void Update() {
        ResetCurrentSelected();
    }

    private void SetInitialObject() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        GameObject go = EventSystem.current.currentSelectedGameObject;
        if (go.GetComponent<Animator>()) {
            go.GetComponent<Animator>().enabled = true;
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
        EnableAnimator();
    }


    private void EnableAnimator() {
        GameObject lastObj = lastSelectedObject;
        if (lastObj && lastObj.GetComponent<Animator>()) {
            lastObj.GetComponent<Animator>().enabled = false;
            lastObj.transform.localScale = new Vector3(1f, 1f, 0f);
        }
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj && currentObj.GetComponent<Animator>()) {
            currentObj.GetComponent<Animator>().enabled = true;
        }
    }
}
