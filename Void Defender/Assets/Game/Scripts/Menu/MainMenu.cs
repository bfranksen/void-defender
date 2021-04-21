﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour {

    [Header("Music Volume")]
    [SerializeField] GameObject sliderContainer;
    [SerializeField] Slider volumeSlider;

    [Header("Buttons")]
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject beforeExitButton;
    [SerializeField] GameObject exitButton;
    [SerializeField] GameObject mvButton;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;
    float buttonScale = 0.8f;

    MusicPlayer musicPlayer;
    float initialMusicVolume;
    float delay = 0.2f;
    float delayLeft = 0f;

    // Start is called before the first frame update
    private void Start() {
#if UNITY_ANDROID || UNITY_IOS
        ReconfigureButtonNav(true);
        RepositionVolumeContainer();
#else
        ReconfigureButtonNav(false);
#endif
        InitialVolumeSettings();
        SetCurrentObject();
    }

    // Update is called once per frame
    private void Update() {
#if !UNITY_ANDROID && !UNITY_IOS
        ResetCurrentSelected();
#endif
        VolumeSliderControl();
    }

    private void ReconfigureButtonNav(bool reconfigure) {
        // Debug.Log(reconfigure ? "Reconfiguring" : "Not Reconfiguring");
        exitButton.SetActive(!reconfigure);
        if (reconfigure) {
            Button btn = beforeExitButton.GetComponent<Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = btn.FindSelectableOnUp();
            nav.selectOnDown = mvButton.GetComponent<Button>();
            btn.navigation = nav;

            btn = mvButton.GetComponent<Button>();
            nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = beforeExitButton.GetComponent<Button>();
            nav.selectOnDown = btn.FindSelectableOnDown();
            btn.navigation = nav;
        }
    }

    private void RepositionVolumeContainer() {
        float safeAreaDiff = Screen.height - Screen.safeArea.height;
        Debug.Log(safeAreaDiff);
        mvButton.transform.parent.gameObject.transform.Translate(new Vector3(42f, safeAreaDiff / -2f - 32f, 0));
        mvButton.transform.parent.gameObject.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    private void InitialVolumeSettings() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        volumeSlider.value = musicPlayer.MusicVolume;
        initialMusicVolume = musicPlayer.MusicVolume;
    }

    private void SetCurrentObject() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
        GameObject go = EventSystem.current.currentSelectedGameObject;
        go.GetComponent<Animator>().enabled = true;
        TextMeshPro tmp = go.GetComponent<TextMeshPro>();
        if (tmp) {
            tmp.color = new Color32(255, 143, 0, 255);
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
            lastObj.transform.localScale = new Vector3(buttonScale, buttonScale, 0);
        }
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj && currentObj.GetComponent<Animator>()) {
            currentObj.GetComponent<Animator>().enabled = true;
        }
    }

    private void VolumeSliderControl() {
        if (delayLeft > 0) {
            delayLeft -= Time.deltaTime;
        }
        if (delayLeft <= 0) {
            if (sliderContainer.activeInHierarchy && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))) {
                initialMusicVolume = volumeSlider.value;
                SaveVolumeChanges(true);
                ShowHideSlider();
            } else if (sliderContainer.activeInHierarchy && Input.GetButtonDown("Cancel")) {
                SaveVolumeChanges(false);
                ShowHideSlider();
            }
        }
    }

    public void AdjustVolume() {
        if (musicPlayer) {
            musicPlayer.MusicVolume = volumeSlider.value;
            musicPlayer.AdjustMusicVolume();
        }
    }

    private void SaveVolumeChanges(bool saveChanges) {
        if (!saveChanges) {
            volumeSlider.value = initialMusicVolume;
            AdjustVolume();
        }
        musicPlayer.SavePlayerPrefs();
    }

    public void ShowHideSlider() {
        delayLeft = delay;
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        if (!sliderContainer.activeInHierarchy) {
            nav = Show(nav);
        } else {
            nav = Hide(nav);
        }
        mvButton.GetComponent<Button>().navigation = nav;
    }

    private Navigation Show(Navigation nav) {
        sliderContainer.SetActive(true);
        nav.selectOnUp = volumeSlider;
        nav.selectOnDown = volumeSlider;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        return nav;
    }

    private Navigation Hide(Navigation nav) {
        sliderContainer.SetActive(false);
#if UNITY_STANDALONE
        nav.selectOnUp = exitButton.GetComponent<Button>();
#else
        nav.selectOnUp = beforeExitButton.GetComponent<Button>();
#endif
        nav.selectOnDown = startButton.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mvButton);
        return nav;
    }
}
