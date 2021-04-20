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
    [SerializeField] GameObject mobilePauseButton;
    [SerializeField] GameObject pauseFirstButton;
    [SerializeField] GameObject optionsFirstButton;
    [SerializeField] GameObject optionsClosedButton;

    [Header("Sliders")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [Header("Utils")]
    [SerializeField] GameObject healthContainer;
    [SerializeField] GameObject livesContainer;
    [SerializeField] GameObject buffContainer;
    [SerializeField] GameObject scoreContainer;
    [SerializeField] GameObject shade;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;

    // General
    public static bool paused = false;
    MusicPlayer musicPlayer;
    float initialMusicVolume;
    float initialSfxVolume;

    // Called before the first update
    private void Start() {
        SetInitialVolumeSettings();
#if UNITY_ANDROID || UNITY_IOS
        SetUpForMobile();
#endif
    }

    // Update is called once per frame
    private void Update() {
        ResetCurrentSelected();
        HandleOpenCloseFromInput();
    }

    private void SetInitialVolumeSettings() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        musicVolumeSlider.value = musicPlayer.MusicVolume * 10f;
        initialMusicVolume = musicPlayer.MusicVolume;
        sfxVolumeSlider.value = musicPlayer.SfxVolume * 10f;
        initialSfxVolume = musicPlayer.SfxVolume;
    }

    private void SetUpForMobile() {
        mobilePauseButton.SetActive(true);
        pauseMenu.transform.localScale = new Vector3(1.25f, 1.25f, 0);
        optionsMenu.transform.localScale = new Vector3(1.25f, 1.25f, 0);
        RepositionUIElements();
    }

    private void RepositionUIElements() {
        Rect safeArea = Screen.safeArea;
        Vector3 topLeftPos = new Vector3(safeArea.xMin, safeArea.yMax, 0);
        Vector3 topRightPos = new Vector3(safeArea.xMax, safeArea.yMax, 0);
        Vector3 botLeftPos = new Vector3(safeArea.xMin, safeArea.yMin, 0);
        Vector3 botRightPos = new Vector3(safeArea.xMax, safeArea.yMin, 0);

        Vector3 topLeftOffset = new Vector3(32f, -32f);
        Vector3 topRightOffset = new Vector3(-32f, -32f);
        Vector3 botLeftOffset = new Vector3(32f, 32f);
        Vector3 botRightOffset = new Vector3(-32f, 32f);
        healthContainer.transform.Translate(topLeftOffset);
        livesContainer.transform.Translate(topLeftOffset);
        buffContainer.transform.Translate(0f, botLeftOffset.y, 0f);
        scoreContainer.transform.Translate(topRightOffset);
        mobilePauseButton.transform.Translate(botLeftOffset);
    }

    private void ResetCurrentSelected() {
        if (EventSystem.current.currentSelectedGameObject != recentSelectedObject) {
            lastSelectedObject = recentSelectedObject;
            recentSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
        if (!EventSystem.current.currentSelectedGameObject) {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
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
        paused = !paused;
        if (paused && !pauseMenu.activeInHierarchy) {
            Movement.pauseCheck = false;
            pauseMenu.SetActive(true);
            shade.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
            Time.timeScale = 0f;
        } else {
            pauseMenu.SetActive(false);
            shade.SetActive(false);
            optionsMenu.SetActive(false);
            Time.timeScale = 1f;
            Input.ResetInputAxes();
            Movement.pauseCheck = true;
        }
    }

    public void RestartGame() {
        PauseUnpause();
        FindObjectOfType<Level>().LoadGame();
    }

    public void OpenOptionsMenu() {
        initialMusicVolume = musicPlayer.MusicVolume;
        initialSfxVolume = musicPlayer.SfxVolume;
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptionsMenu() {
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void ConfirmOptions() {
        SaveVolumeChanges(true);
        CloseOptionsMenu();
    }

    public void CancelOptions() {
        SaveVolumeChanges(false);
        CloseOptionsMenu();
    }

    public void QuitGame() {
        PauseUnpause();
        FindObjectOfType<Level>().LoadStartMenu();
    }

    public void AdjustVolumeFromSliderChange(Slider slider) {
        if (slider == musicVolumeSlider) {
            musicPlayer.MusicVolume = musicVolumeSlider.value / 10f;
        } else if (slider == sfxVolumeSlider) {
            musicPlayer.SfxVolume = sfxVolumeSlider.value / 10f;
        }
        musicPlayer.AdjustMusicVolume();
    }

    private void SaveVolumeChanges(bool saveChanges) {
        if (!saveChanges) {
            musicVolumeSlider.value = initialMusicVolume * 10f;
            sfxVolumeSlider.value = initialSfxVolume * 10f;
            musicPlayer.MusicVolume = initialMusicVolume;
            musicPlayer.SfxVolume = initialSfxVolume;
            musicPlayer.AdjustMusicVolume();
        }
        musicPlayer.SavePlayerPrefs();
    }
}
