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
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject mvButton;

    MusicPlayer musicPlayer;
    float initialMusicVolume;
    float delay = 0.2f;
    float delayLeft = 0f;

    // Start is called before the first frame update
    private void Start() {
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
        if (delayLeft > 0) {
            delayLeft -= Time.deltaTime;
        }
        if (delayLeft <= 0) {
            AdjustVolume(true);
            if (sliderContainer.activeInHierarchy && Input.GetButtonDown("Fire1")) {
                initialMusicVolume = volumeSlider.value;
                ShowHideSlider();
            } else if (sliderContainer.activeInHierarchy && Input.GetButtonDown("Cancel")) {
                ShowHideSlider();
                AdjustVolume(false);
            }
        }
    }

    private void AdjustVolume(bool saveChanges) {
        if (saveChanges) {
            musicPlayer.MusicVolume = volumeSlider.value;
            musicPlayer.AdjustMusicVolume();
        } else {
            volumeSlider.value = initialMusicVolume;
            PlayerPrefs.SetFloat(MusicPlayer.MUSIC_VOLUME_KEY, initialMusicVolume);
        }
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
        nav.selectOnUp = quitButton.GetComponent<Button>();
        nav.selectOnDown = startButton.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mvButton);
        return nav;
    }
}
