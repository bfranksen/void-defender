using System.Collections;
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
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject beforeExitButton;
    [SerializeField] GameObject exitButton;
    [SerializeField] GameObject mvButton;
    [SerializeField] GameObject accountButton;
    [SerializeField] GameObject firstAccountButton;
    [SerializeField] GameObject lastAccountButton;


    [Header("Account Menu")]
    [SerializeField] GameObject accountMenu;
    [SerializeField] GameObject accountDropdownGO;
    [SerializeField] TMP_Dropdown accountDropdown;
    [SerializeField] GameObject accountCreateUsernameGO;
    [SerializeField] TMP_InputField accountCreateInput;
    [SerializeField] TextMeshProUGUI accountCreateValidCheck;
    [SerializeField] GameObject shade;
    bool validUsername = false;

    GameObject recentSelectedObject;
    GameObject lastSelectedObject;
    float buttonScale = 1.0f;

    MusicPlayer musicPlayer;
    float initialMusicVolume;
    float delay = 0.2f;
    float delayLeft = 0f;

    // Start is called before the first frame update
    private void Start() {
#if UNITY_ANDROID || UNITY_IOS
        ReconfigureButtonNav(true);
        ChangeButtonColorScheme();
#else
        ReconfigureButtonNav(false);
#endif
        RepositionVolumeContainer();
        InitialVolumeSettings();
        SetInitialObject();
    }

    // Update is called once per frame
    private void Update() {
#if !UNITY_ANDROID && !UNITY_IOS
        ResetCurrentSelected();
#endif
        VolumeSliderControl();
    }

    private void ReconfigureButtonNav(bool reconfigure) {
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

    private void RepositionVolumeContainer() {
        float safeAreaDiff = Screen.height - Screen.safeArea.yMax;
        mvButton.transform.parent.gameObject.transform.Translate(new Vector3(safeAreaDiff / 2f + 16f, safeAreaDiff / -2f - 16f, 0));
        mvButton.transform.parent.gameObject.transform.localScale = new Vector2(1.25f, 1.25f);

        accountButton.transform.parent.gameObject.transform.Translate(new Vector3(safeAreaDiff / 2f - 16f, safeAreaDiff / -2f - 16f, 0));
        accountButton.transform.parent.gameObject.transform.localScale = new Vector2(1.25f, 1.25f);
    }

    private void InitialVolumeSettings() {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        volumeSlider.value = musicPlayer.MusicVolume;
        initialMusicVolume = musicPlayer.MusicVolume;
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
        nav.selectOnDown = firstButton.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mvButton);
        return nav;
    }

    public void ShowHideAccountMenu() {
        if (!accountMenu.activeInHierarchy) {
            GetUsernames();
            SizeElementsInMenu(accountMenu.gameObject);
            shade.SetActive(true);
            accountMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstAccountButton);
        } else {
            accountMenu.SetActive(false);
            shade.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    public void ConfirmAccountChoices() {
        if (accountCreateInput.text != "" && validUsername) {
            PlayerPrefsController.AttemptToAddUsername(accountCreateInput.text);
        }
        accountCreateInput.text = "";
        ShowHideAccountMenu();
    }

    public void CancelAccountChoices() {
        if (accountCreateInput.text != "") {
            accountCreateInput.text = "";
        }
        if (accountDropdownGO.activeInHierarchy && accountDropdown.options.Count > 0) {
            if (PlayerPrefsController.GetCurrentUserAccount() != "") {
                accountDropdown.value = PlayerPrefsController.currentAccountIndex;
            } else {
                accountDropdown.value = 0;
            }
        }
    }

    private void GetUsernames() {
        List<string> usernames = PlayerPrefsController.GetUserAccounts();
        if (usernames.Count <= 0) {
            accountDropdownGO.SetActive(false);
            accountCreateUsernameGO.gameObject.transform.position += new Vector3(0f, -128f, 0f);
        } else if (usernames.Count >= 5) {
            accountCreateUsernameGO.SetActive(false);
            accountDropdownGO.gameObject.transform.position += new Vector3(04, 128f, 0f);
            accountDropdown.options.Clear();
            for (int i = 0; i < usernames.Count; i++) {
                accountDropdown.options.Add(new TMP_Dropdown.OptionData() { text = usernames[i] });
            }
        } else {
            accountDropdown.options.Clear();
            for (int i = 0; i < usernames.Count; i++) {
                accountDropdown.options.Add(new TMP_Dropdown.OptionData() { text = usernames[i] });
            }
        }
        if (usernames.Count <= 0) {
            ReconfigureAccountButtonNav(true);
        } else if (usernames.Count >= 5) {
            ReconfigureAccountButtonNav(false);
        }
    }

    private void ReconfigureAccountButtonNav(bool noAccounts) {
        Selectable btnTop;
        Button btnBot = lastAccountButton.GetComponent<Button>();
        if (noAccounts) {
            btnTop = accountCreateUsernameGO.GetComponentInChildren<TMP_InputField>();
        } else {
            btnTop = accountDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        }
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = btnBot.FindSelectableOnUp();
        nav.selectOnDown = btnTop;
        btnBot.navigation = nav;

        nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = btnBot;
        nav.selectOnDown = firstAccountButton.GetComponent<Button>();
        btnTop.navigation = nav;

        nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = btnTop;
        nav.selectOnDown = firstAccountButton.GetComponent<Button>();
        firstAccountButton.GetComponent<Button>().navigation = nav;
    }

    public void CheckForUsernameExistence() {
        if (!accountCreateValidCheck.gameObject.activeInHierarchy) {
            accountCreateValidCheck.gameObject.SetActive(true);
        }
        string username = accountCreateInput.text;
        if (username.Length >= 3) {
            StartCoroutine(RunCheck(username));
        } else {
            accountCreateValidCheck.text = "Username must be between 3 and 16 characters.";
        }
    }

    private IEnumerator RunCheck(string username) {
        bool localCheck = !PlayerPrefsController.CheckForUsernameExistence(username);
        HighScores hs = FindObjectOfType<HighScores>();
        yield return StartCoroutine(hs.CheckUsernameExists(username));
        bool globalCheck = !hs.usernameExists;

        if (localCheck && globalCheck) {
            accountCreateValidCheck.text = "Username is valid. It will be created if you hit Confirm.";
            validUsername = true;
        } else {
            accountCreateValidCheck.text = "Username is already taken. Try again.";
            validUsername = false;
        }
    }

    private void SizeElementsInMenu(GameObject go) {
        Vector2 rtSizeDelta = new Vector2(Screen.width / -4f, Screen.height / -4f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = rtSizeDelta;
    }
}
