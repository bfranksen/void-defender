using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManualMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject previousPageButton;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;

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
        if (Input.GetButtonDown("Cancel")) {
            previousPageButton.GetComponent<Button>().onClick.Invoke();
        }
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
            if (lastObj.GetComponentInChildren<Image>()) {
                lastObj.GetComponentInChildren<Image>().color = Color.white;
            }
        }
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj && currentObj.GetComponent<Animator>()) {
            currentObj.GetComponent<Animator>().enabled = true;
            if (currentObj.GetComponentInChildren<Image>()) {
                currentObj.GetComponentInChildren<Image>().color = new Color(1, 0.5615011f, 0);
            }
        }
    }
}
