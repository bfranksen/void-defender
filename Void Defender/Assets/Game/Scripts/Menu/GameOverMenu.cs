using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;

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
        if (!EventSystem.current.currentSelectedGameObject &&
            (Input.GetButtonDown("Vertical") || Input.GetButtonDown("Horizontal"))) {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }
}
