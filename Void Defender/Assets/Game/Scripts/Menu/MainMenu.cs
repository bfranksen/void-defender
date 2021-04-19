using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
#if UNITY_STANDALONE
        ReconfigureButtonNav(false);
#else
        ReconfigureButtonNav(true);
#endif
        musicPlayer = FindObjectOfType<MusicPlayer>();
        volumeSlider.value = musicPlayer.MusicVolume;
        initialMusicVolume = musicPlayer.MusicVolume;
        if (Input.GetJoystickNames().Length > 0) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        } else {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Update is called once per frame
    private void Update() {
        ResetCurrentSelected();
        VolumeSliderControl();
    }

    private void ReconfigureButtonNav(bool reconfigure) {
        Debug.Log(reconfigure ? "Reconfiguring" : "Not Reconfiguring");
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
