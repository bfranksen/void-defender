using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManualMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject previousPageButton;

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
    }
}
