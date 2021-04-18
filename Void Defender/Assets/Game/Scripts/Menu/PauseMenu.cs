using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;

    [Header("Buttons")]
    [SerializeField] GameObject pauseFirstButton;
    [SerializeField] GameObject optionsFirstButton;
    [SerializeField] GameObject optionsClosedButton;

    [Header("Sliders")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [Header("Utils")]
    [SerializeField] GameObject shade;

    // General
    MusicPlayer musicPlayer;
    float initialMusicVolume;
    float initialSfxVolume;
    bool keepInitialVolume;

    // Called before the first update
    private void Start() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        musicVolumeSlider.value = musicPlayer.MusicVolume * 10f;
        initialMusicVolume = musicPlayer.MusicVolume;
        sfxVolumeSlider.value = musicPlayer.SfxVolume * 10f;
        initialSfxVolume = musicPlayer.SfxVolume;
    }

    // Update is called once per frame
    private void Update() {
        if (musicPlayer.MusicVolume != musicVolumeSlider.value || musicPlayer.SfxVolume != sfxVolumeSlider.value) {
            UpdateVolume();
        }
        ResetCurrentSelected();
        HandleOpenCloseFromInput();
    }

    private void ResetCurrentSelected() {
        if (optionsMenu.activeInHierarchy && !EventSystem.current.currentSelectedGameObject && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))) {
            EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        } else if (!EventSystem.current.currentSelectedGameObject && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))) {
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
    }

    private void HandleOpenCloseFromInput() {
        if (!optionsMenu.activeInHierarchy && (Input.GetButtonDown("Pause") || (pauseMenu.activeInHierarchy && Input.GetButtonDown("Cancel")))) {
            PauseUnpause();
        } else if (optionsMenu.activeInHierarchy && (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel"))) {
            CancelOptions();
        }
    }

    public void PauseUnpause() {
        if (!pauseMenu.activeInHierarchy) {
            pauseMenu.SetActive(true);
            shade.SetActive(true);
            Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        } else {
            pauseMenu.SetActive(false);
            shade.SetActive(false);
            optionsMenu.SetActive(false);
            Time.timeScale = 1f;
            Input.ResetInputAxes();
        }
    }

    public void RestartGame() {
        PauseUnpause();
        FindObjectOfType<Level>().LoadGame();
    }

    public void OpenOptionsMenu() {
        initialMusicVolume = musicPlayer.MusicVolume;
        initialSfxVolume = musicPlayer.SfxVolume;
        keepInitialVolume = false;
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptionsMenu() {
        UpdateVolume();
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void ConfirmOptions() {
        CloseOptionsMenu();
    }

    public void CancelOptions() {
        keepInitialVolume = true;
        CloseOptionsMenu();
    }

    private void UpdateVolume() {
        musicPlayer.MusicVolume = keepInitialVolume ? initialMusicVolume : musicVolumeSlider.value / 10f;
        musicPlayer.SfxVolume = keepInitialVolume ? initialSfxVolume : sfxVolumeSlider.value / 10f;
        musicPlayer.AdjustMusicVolume();
        musicVolumeSlider.value = musicPlayer.MusicVolume * 10f;
        sfxVolumeSlider.value = musicPlayer.SfxVolume * 10f;
    }

    public void QuitGame() {
        PauseUnpause();
        FindObjectOfType<Level>().LoadStartMenu();
    }
}
