using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ManualMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject previousPageButton;

    [Header("Controls")]
    [SerializeField] GameObject keyboardControls;
    [SerializeField] GameObject touchControls;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;

    // Start is called before the first frame update
    private void Start() {
        SetInitialObject();
        RepositionElements();
        if (keyboardControls && touchControls) UpdateControlsVisibility();
#if UNITY_ANDROID || UNITY_IOS
        ChangeButtonColorScheme();
#endif
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            previousPageButton.GetComponent<Button>().onClick.Invoke();
        }
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

    private void RepositionElements() {
        if (Screen.height - Screen.safeArea.height > 0 || Camera.main.aspect < 0.52f) {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            float screenHeight = Screen.height;
            float safeAreaHeight = Screen.safeArea.height;
            float safeAreaYMax = Screen.safeArea.yMax;
            float childOneY = rt.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition.y;
            float childTwoY = rt.GetChild(3).gameObject.GetComponent<RectTransform>().anchoredPosition.y;
            float childOneOffset = screenHeight - safeAreaYMax + childOneY;
            float childTwoOffset = screenHeight - safeAreaHeight - (screenHeight - safeAreaYMax) + childTwoY;
            rt.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, childOneOffset < 0 ? -childOneOffset : childOneOffset);
            rt.GetChild(3).gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, childTwoOffset * 0.6f);
            // rt.GetChild(1).gameObject.GetComponent<RectTransform>().transform.position -= new Vector3(0, Screen.height - Screen.safeArea.yMax - rt.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition.y, 0);
            // rt.GetChild(3).gameObject.GetComponent<RectTransform>().transform.position += new Vector3(0, Screen.height - Screen.safeArea.height - (Screen.height - Screen.safeArea.yMax) + rt.GetChild(3).gameObject.GetComponent<RectTransform>().anchoredPosition.y, 0);
        }
    }
    private void UpdateControlsVisibility() {
#if UNITY_ANDROID || UNITY_IOS
        touchControls.SetActive(true);
        keyboardControls.SetActive(false);
#else
        touchControls.SetActive(false);
        keyboardControls.SetActive(true);
#endif
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

    private void SetSizeDeltas() {
        Vector2 rtSizeDelta = new Vector2(Screen.width / -8f, 0);
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        List<GameObject> children = new List<GameObject>();
        for (int i = 1; i < rt.transform.childCount; i++) {
            GameObject child = rt.transform.GetChild(i).gameObject;
            RectTransform childRt = child.GetComponent<RectTransform>();
            if (i != 2) {
                childRt.sizeDelta += rtSizeDelta;
            }
            children.Add(child);
            if (i == 3 && Camera.main.aspect < 9f / 16f) {
                PositionChildren(children, Screen.safeArea);
            }
        }
    }

    private void PositionChildren(List<GameObject> list, Rect safeArea) {
        list[1].transform.position = new Vector2(0, safeArea.center.y - safeArea.height * 0.072918f);
        float heightAvailable = safeArea.height - safeArea.height * 0.572918f;

        RectTransform firstRt = list[0].GetComponent<RectTransform>();
        float newYMax = (heightAvailable * 2 / 3 - firstRt.rect.height) / 2;
        list[0].transform.position = new Vector2(list[0].GetComponent<RectTransform>().sizeDelta.x, safeArea.yMax - newYMax);
        RectTransform secondRt = list[2].GetComponent<RectTransform>();
        newYMax = (heightAvailable * 2 / 3 - secondRt.rect.height) / 2;
        list[2].transform.position = new Vector2(list[2].GetComponent<RectTransform>().sizeDelta.x, safeArea.yMin + newYMax);
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
