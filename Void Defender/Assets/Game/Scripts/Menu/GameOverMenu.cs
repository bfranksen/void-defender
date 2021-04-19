using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;
    float buttonScale = 0.8f;

    // Start is called before the first frame update
    private void Start() {
        if (Input.GetJoystickNames().Length > 0) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        } else {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Update is called once per frame
    private void Update() {
        ResetCurrentSelected();
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
            lastObj.transform.localScale = new Vector3(buttonScale, buttonScale, 0);
        }
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj && currentObj.GetComponent<Animator>()) {
            currentObj.GetComponent<Animator>().enabled = true;
        }
    }
}
