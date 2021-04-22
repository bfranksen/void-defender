using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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

    [Header("Dropdown")]
    [SerializeField] TMP_Dropdown movementDropdown;
    [SerializeField] TextMeshProUGUI movementDropdownTooltip;

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
    Movement movement;
    int initialMoveMode;

    // Called before the first update
    private void Start() {
        SetInitialVolumeSettings();
#if UNITY_ANDROID || UNITY_IOS
        SetUpForMobile();
        StartCoroutine(SetMovementDropdown());
#else
        mobilePauseButton.SetActive(false);
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
        SizeElementsInMenu(pauseMenu, true);
        SizeElementsInMenu(optionsMenu, false);
        RepositionUIElements();
    }

    private IEnumerator SetMovementDropdown() {
        movement = FindObjectOfType<Movement>();
        initialMoveMode = -1;
        yield return new WaitForSeconds(0.1f);
        movementDropdown.value = (int)movement.TouchConfig;
    }

    private void SizeElementsInMenu(GameObject go, bool isPauseMenu) {
        Vector2 rtSizeDelta = new Vector2(Screen.width / -6f, Screen.height / -4f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = rtSizeDelta;

        float width = rt.rect.width;
        float height = rt.rect.height;
        float runningHeight = height * -0.025f;
        Vector2 childSizeDelta = new Vector2(width / -6f, 0);
        int numChildren = rt.transform.childCount;
        for (int i = 0; i < numChildren - 1; i++) {
            GameObject child = rt.transform.GetChild(i).gameObject;
            RectTransform childRt = child.GetComponent<RectTransform>();
            if (i > 0 && isPauseMenu) {
                runningHeight += height * -0.12f;
            } else if (i > 0 && i < 4 && !isPauseMenu) {
                runningHeight += height * -0.15f;
            } else if (i > 0) {
                if (i == 4) runningHeight = -height + height * .25f;
                if (i != 4) runningHeight -= height * 0.12f;
            }
            childRt.anchoredPosition = new Vector2(0, runningHeight);
            childRt.sizeDelta += childSizeDelta;
            if (i == 0 && isPauseMenu) runningHeight += height * -0.15f;
            else if (i == 0) runningHeight += height * -0.05f;
        }
    }

    private void RepositionUIElements() {
        Rect safeArea = Screen.safeArea;
        Vector3 topLeftPos = new Vector3(safeArea.xMin, safeArea.yMax, 0);
        Vector3 topRightPos = new Vector3(safeArea.xMax, safeArea.yMax, 0);
        Vector3 botLeftPos = new Vector3(safeArea.xMin, safeArea.yMin, 0);
        Vector3 botRightPos = new Vector3(safeArea.xMax, safeArea.yMin, 0);
        float offset = safeArea.width / 32f;
        Vector3 topLeftOffset = new Vector3(offset, -offset, 0f);
        Vector3 topRightOffset = new Vector3(-offset, -offset, 0f);
        Vector3 botLeftOffset = new Vector3(offset, offset, 0f);
        Vector3 botRightOffset = new Vector3(-offset, offset, 0f);
        healthContainer.transform.Translate(topLeftOffset);
        livesContainer.transform.Translate(topLeftOffset.x, topLeftOffset.y * 2f, 0f);
        scoreContainer.transform.Translate(topRightOffset);
        buffContainer.transform.Translate(0f, botLeftOffset.y * 5f, 0f);
        mobilePauseButton.transform.Translate(botLeftOffset * 0.75f);
    }

    public void UpdateDropdownTooltip() {
        int dropdownValue = movementDropdown.value;
        if (initialMoveMode == -1) {
            initialMoveMode = dropdownValue;
        }
        string[] tooltips = { // Movement mode tooltips
            "Follows your touch", // Follow
            "Stationary joystick", // Fixed Joystick
            "Joystick that moves with you" // Dynamic Joystick
            };
        movementDropdownTooltip.text = tooltips[dropdownValue];
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
        Player player = FindObjectOfType<Player>();
        if (player && !player.CanFire) {
            return;
        }
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
        if (!FindObjectOfType<Player>().CanFire) {
            return;
        }
        initialMusicVolume = musicPlayer.MusicVolume;
        initialSfxVolume = musicPlayer.SfxVolume;
        initialMoveMode = (int)movement.TouchConfig;
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
        SaveMovementModeChanges(true);
        CloseOptionsMenu();
    }

    public void CancelOptions() {
        SaveVolumeChanges(false);
        SaveMovementModeChanges(false);
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

    private void SaveMovementModeChanges(bool saveChanges) {
        if (saveChanges) {
            movement.SetMovementModePref(movementDropdown.value);
        } else {
            movementDropdown.value = initialMoveMode;
        }
    }
}
