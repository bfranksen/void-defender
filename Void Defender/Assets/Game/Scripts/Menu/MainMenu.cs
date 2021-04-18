using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Sliders")]
    [SerializeField] GameObject sliderContainer;
    [SerializeField] Slider musicVolumeSlider;

    [Header("Buttons")]
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject selectedButton;
    [SerializeField] GameObject unselectedButton;

    MusicPlayer musicPlayer;

    // Start is called before the first frame update
    private void Start() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        musicVolumeSlider.value = musicPlayer.MusicVolume;
    }

    // Update is called once per frame
    private void Update() {
        musicPlayer.MusicVolume = musicVolumeSlider.value;
        musicPlayer.AdjustMusicVolume();
    }

    public void ShowSlider() {
        if (!sliderContainer.activeInHierarchy) {
            sliderContainer.SetActive(true);
            unselectedButton.SetActive(false);
            selectedButton.SetActive(true);
        } else {
            sliderContainer.SetActive(false);
            unselectedButton.SetActive(true);
            selectedButton.SetActive(false);
        }
    }
}
