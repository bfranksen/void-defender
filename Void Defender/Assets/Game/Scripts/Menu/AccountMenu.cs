using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AccountMenu : MonoBehaviour {

    [Header("Buttons")]
    [SerializeField] GameObject firstProfilesMenuButton;
    [SerializeField] GameObject firstNewProfileMenuButton;
    [SerializeField] GameObject closedProfilesMenuButton;


    [Header("Menus")]
    [SerializeField] GameObject profilesMenu;
    [SerializeField] GameObject newProfileMenu;


    [Header("Profiles Menu")]
    [SerializeField] TextMeshProUGUI needProfileText;
    [SerializeField] List<GameObject> userAccounts;
    [SerializeField] GameObject switchGo;
    [SerializeField] GameObject deleteGo;
    [SerializeField] GameObject alreadyGo;


    [Header("New Profile Menu")]
    // [SerializeField] GameObject accountCreateUsernameGO;
    [SerializeField] TMP_InputField profileCreateInput;
    [SerializeField] TextMeshProUGUI profileCreateValidCheck;

    [Header("General")]
    [SerializeField] GameObject shade;

    // General
    Color32 appOrange = new Color32(255, 143, 0, 255);
    bool validUsername = false;
    int selectedUserIndex;
    GameObject lastSelectedObject;
    GameObject recentSelectedObject;
    Coroutine usernameCheckCoroutine;
    Coroutine alreadySelectedCoroutine;

    private void Update() {
        // ResetCurrentSelected();
        HandleOpenCloseFromInput();
    }

    // private void ResetCurrentSelected() {
    //     GameObject selected = EventSystem.current.currentSelectedGameObject;
    //     if (selected != recentSelectedObject) {
    //         lastSelectedObject = recentSelectedObject;
    //         recentSelectedObject = selected;
    //     }
    //     if (!selected) {
    //         EventSystem.current.SetSelectedGameObject(lastSelectedObject);
    //     }
    // }

    private void HandleOpenCloseFromInput() {
        if (!newProfileMenu.activeInHierarchy && (Input.GetButtonDown("Pause") || (profilesMenu.activeInHierarchy && Input.GetButtonDown("Cancel")))) {
            ShowHideProfilesMenu();
        } else if (newProfileMenu.activeInHierarchy && (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel"))) {
            ShowHideNewProfileMenu();
        }
    }

    public void ShowHideProfilesMenu() {
        FindObjectOfType<LocalHighScoreDisplay>().Refresh();
        if (!profilesMenu.activeInHierarchy) {
            shade.SetActive(true);
            profilesMenu.SetActive(true);
            GetUsernames();
            ReconfigureButtonNav();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstProfilesMenuButton);
        } else {
            profilesMenu.SetActive(false);
            shade.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(closedProfilesMenuButton);
        }
        if (switchGo.activeInHierarchy) {
            CancelSelect();
        }
        if (deleteGo.activeInHierarchy) {
            CancelDelete();
        }
        ResetAlreadySelectedCoroutine();
    }

    public void ShowHideNewProfileMenu() {
        if (profilesMenu.activeInHierarchy && !newProfileMenu.activeInHierarchy) {
            profilesMenu.SetActive(false);
            newProfileMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstNewProfileMenuButton);
        } else {
            profileCreateInput.text = "";
            profilesMenu.SetActive(true);
            newProfileMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstProfilesMenuButton);
        }
        if (switchGo.activeInHierarchy) {
            CancelSelect();
        }
        if (deleteGo.activeInHierarchy) {
            CancelDelete();
        }
        ResetAlreadySelectedCoroutine();
    }

    private void GetUsernames() {
        // Debug.Log("Current Account: " + PlayerPrefsController.GetCurrentUserAccount() + "  -  Index: " + PlayerPrefsController.GetCurrentUserIndex());
        List<string> usernames = PlayerPrefsController.GetUserAccounts();
        if (usernames.Count <= 0) {
            needProfileText.gameObject.SetActive(true);
            if (userAccounts[0].activeInHierarchy) {
                userAccounts[0].SetActive(false);
            }
        } else {
            needProfileText.gameObject.SetActive(false);
            for (int i = 0; i < 5; i++) {
                if (i < usernames.Count) {
                    userAccounts[i].SetActive(true);
                    userAccounts[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = usernames[i];
                    if (i == PlayerPrefsController.GetCurrentUserIndex()) {
                        userAccounts[i].transform.GetChild(0).gameObject.SetActive(true);
                        userAccounts[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = (Color)appOrange;
                    } else {
                        userAccounts[i].transform.GetChild(0).gameObject.SetActive(false);
                        userAccounts[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;
                    }
                } else {
                    userAccounts[i].SetActive(false);
                }
            }
        }
        Button[] buttons = profilesMenu.GetComponentsInChildren<Button>();
        if (usernames.Count >= 5) {
            buttons[buttons.Length - 2].interactable = false;
        } else {
            buttons[buttons.Length - 2].interactable = true;
        }
        ReconfigureButtonNav();
    }

    private void ReconfigureButtonNav() {
        List<string> usernames = PlayerPrefsController.GetUserAccounts();
        Button[] buttons = profilesMenu.GetComponentsInChildren<Button>();
        Navigation nav;

        if (usernames.Count == 0) {
            nav = buttons[0].navigation;
            nav.selectOnUp = buttons[1];
            nav.selectOnDown = buttons[1];
            buttons[0].navigation = nav;
            nav = buttons[1].navigation;
            nav.selectOnUp = buttons[0];
            nav.selectOnDown = buttons[0];
            buttons[1].navigation = nav;
        } else {
            nav = buttons[buttons.Length - 1].navigation;
            nav.selectOnDown = buttons[0];
            buttons[buttons.Length - 1].navigation = nav;

            nav = buttons[buttons.Length - 2].navigation;
            nav.selectOnUp = buttons[usernames.Count * 2 - 2];
            buttons[buttons.Length - 2].navigation = nav;

            nav = buttons[usernames.Count * 2 - 2].navigation;
            nav.selectOnDown = buttons[buttons.Length - 2];
            buttons[usernames.Count * 2 - 2].navigation = nav;

            nav = buttons[usernames.Count * 2 - 1].navigation;
            nav.selectOnDown = buttons[buttons.Length - 2];
            buttons[usernames.Count * 2 - 1].navigation = nav;
        }

        if (!EventSystem.current.alreadySelecting) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[buttons.Length - 2].gameObject);
        }
    }

    private void ReconfigureButtonNavConfirm() {
        Button[] buttons = profilesMenu.GetComponentsInChildren<Button>();
        Navigation nav;

        buttons[0].interactable = false;
        buttons[1].interactable = false;

        nav = buttons[buttons.Length - 4].navigation; // Confirm button
        nav.selectOnUp = buttons[buttons.Length - 1]; // Return to Menu button
        nav.selectOnDown = buttons[buttons.Length - 2]; // New Profile button     
        buttons[buttons.Length - 4].navigation = nav;// Set Confirm button nav

        nav = buttons[buttons.Length - 3].navigation; // Cancel button
        nav.selectOnUp = buttons[buttons.Length - 1]; // Return to Menu button
        nav.selectOnDown = buttons[buttons.Length - 2]; // New Profile button
        buttons[buttons.Length - 3].navigation = nav; // Set Cancel button nav

        nav = buttons[buttons.Length - 2].navigation; // New Profile button
        nav.selectOnUp = buttons[buttons.Length - 4]; // Confirm button
        buttons[buttons.Length - 2].navigation = nav; // Set New Profile button nav

        nav = buttons[buttons.Length - 1].navigation; // Return to Menu button
        nav.selectOnDown = buttons[buttons.Length - 4]; // Confirm button
        buttons[buttons.Length - 1].navigation = nav; // Set Return to Menu button nav

        if (!EventSystem.current.alreadySelecting) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[buttons.Length - 4].gameObject);
        }
    }

    public void AttemptToAddUserName() {
        if (usernameCheckCoroutine == null) {
            StartCoroutine(AddUserName());
        }
    }

    private IEnumerator AddUserName() {
        firstNewProfileMenuButton.GetComponent<Button>().interactable = false;
        bool added = false;
        yield return StartCoroutine(CheckForUsernameExistence());
        string attemptedUsername = profileCreateInput.text;
        if (validUsername) {
            profileCreateValidCheck.text = "Creating...";
            added = PlayerPrefsController.AttemptToAddUsername(attemptedUsername);
        }
        yield return new WaitForSeconds(1.0f);
        if (added) {
            profileCreateInput.text = "";
            profileCreateValidCheck.text = "";
            ShowHideNewProfileMenu();
            GetUsernames();
        }
        firstNewProfileMenuButton.GetComponent<Button>().interactable = true;
    }

    public void CheckUsername() {
        if (usernameCheckCoroutine != null) {
            StopCoroutine(usernameCheckCoroutine);
        }
        usernameCheckCoroutine = StartCoroutine(WaitForEditToEnd());
    }

    private IEnumerator WaitForEditToEnd() {
        firstNewProfileMenuButton.GetComponent<Button>().interactable = false;
        if (!profileCreateValidCheck.gameObject.activeInHierarchy) {
            profileCreateValidCheck.gameObject.SetActive(true);
        }
        profileCreateValidCheck.text = "Checking...";
        yield return new WaitForSeconds(1.33f);
        yield return StartCoroutine(CheckForUsernameExistence());
        usernameCheckCoroutine = null;
        firstNewProfileMenuButton.GetComponent<Button>().interactable = true;
    }

    private IEnumerator CheckForUsernameExistence() {
        if (!profileCreateValidCheck.gameObject.activeInHierarchy) {
            profileCreateValidCheck.gameObject.SetActive(true);
        }
        string username = profileCreateInput.text;
        if (username.Length >= 3) {
            yield return StartCoroutine(RunCheck(username));
        } else {
            profileCreateValidCheck.text = "Username must be between 3 and 16 characters.";
        }
    }

    private IEnumerator RunCheck(string username) {
        bool localCheck = !PlayerPrefsController.CheckForUsernameExistence(username);
        HighScores hs = FindObjectOfType<HighScores>();
        yield return StartCoroutine(hs.CheckUsernameExists(username));
        bool globalCheck = !hs.usernameExists;
        // Debug.Log("Local Check: " + localCheck + "  -  Global Check: " + globalCheck);
        if (localCheck && globalCheck) {
            profileCreateValidCheck.text = "Valid username. It will be created if you hit confirm.";
            validUsername = true;
        } else {
            profileCreateValidCheck.text = "Username is already taken. Try again.";
            validUsername = false;
        }
    }

    // public void SetCurrentAccountColors(int numUsers) {
    //     for (int i = 0; i < numUsers; i++) {
    //         if (i == PlayerPrefsController.GetCurrentUserIndex()) {
    //             ColorSwitch(userAccounts[i], true);
    //         } else {
    //             ColorSwitch(userAccounts[i], false);
    //         }
    //     }
    // }

    // private void ColorSwitch(GameObject account, bool orangeNeeded) {
    //     account.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = orangeNeeded ? (Color)appOrange : Color.white;
    //     account.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = orangeNeeded ? (Color)appOrange : Color.white;

    //     ColorBlock colors = new ColorBlock();
    //     colors.normalColor = orangeNeeded ? (Color)appOrange : Color.white;
    //     colors.highlightedColor = orangeNeeded ? Color.white : (Color)appOrange;
    //     colors.selectedColor = orangeNeeded ? Color.white : (Color)appOrange;
    //     colors.pressedColor = orangeNeeded ? Color.white : (Color)appOrange;
    //     colors.disabledColor = Color.grey;
    //     colors.colorMultiplier = 1f;
    //     account.transform.GetChild(2).GetComponent<Button>().colors = colors;
    //     account.transform.GetChild(3).GetComponent<Button>().colors = colors;
    // }

    public void ShowConfirmSelect(int index) {
        if (index == PlayerPrefsController.GetCurrentUserIndex()) {
            if (alreadySelectedCoroutine == null) {
                alreadySelectedCoroutine = StartCoroutine(ShowAlreadySelectedText());
            }
            ResetSelectedObject();
            return;
        } else {
            ResetAlreadySelectedCoroutine();
        }
        selectedUserIndex = index;
        for (int i = 0; i < 5; i++) {
            if (i != index && userAccounts[i].activeInHierarchy) {
                userAccounts[i].SetActive(false);
            }
        }
        switchGo.SetActive(true);
        ReconfigureButtonNavConfirm();
    }

    public void ConfirmSelect() {
        ResetInteractables();
        Debug.Log("Switched to profile at index " + selectedUserIndex);
        FindObjectOfType<LocalHighScoreDisplay>().Refresh();
        PlayerPrefsController.SetCurrentUserIndex(selectedUserIndex);
        switchGo.SetActive(false);
        GetUsernames();
    }

    public void CancelSelect() {
        ResetInteractables();
        switchGo.SetActive(false);
        GetUsernames();
    }

    public void ShowConfirmDelete(int index) {
        ResetAlreadySelectedCoroutine();
        selectedUserIndex = index;
        for (int i = 0; i < 5; i++) {
            if (i != index && userAccounts[i].activeInHierarchy) {
                userAccounts[i].SetActive(false);
            }
        }
        deleteGo.SetActive(true);
        ReconfigureButtonNavConfirm();
    }

    public void ConfirmDelete() {
        ResetInteractables();
        if (PlayerPrefsController.DeleteUserAccount(selectedUserIndex)) {
            Debug.Log("Account at index " + selectedUserIndex + " deleted successfully");
        } else {
            Debug.Log("Account at index " + selectedUserIndex + " not deleted");
        }
        deleteGo.SetActive(false);
        GetUsernames();
    }

    public void CancelDelete() {
        ResetInteractables();
        deleteGo.SetActive(false);
        GetUsernames();
    }

    private void ResetInteractables() {
        Button[] buttons = profilesMenu.GetComponentsInChildren<Button>();
        buttons[0].interactable = true;
        buttons[1].interactable = true;
    }

    private IEnumerator ShowAlreadySelectedText() {
        alreadyGo.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        ResetAlreadySelectedCoroutine();
    }

    private void ResetAlreadySelectedCoroutine() {
        if (alreadySelectedCoroutine != null) {
            alreadySelectedCoroutine = null;
            alreadyGo.SetActive(false);
        }
    }

    public void ResetSelectedObject() {
        if (EventSystem.current.currentSelectedGameObject != firstProfilesMenuButton) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstProfilesMenuButton);
        }
    }
}
