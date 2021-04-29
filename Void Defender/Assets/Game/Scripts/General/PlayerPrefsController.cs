using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPrefsController {

    // Keys
    private static List<string> USER_ACCOUNT_KEYS = new List<string>();
    private const string CURRENT_USER_KEY = "currentUser";
    private const string MUSIC_VOLUME_KEY = "musicVol";
    private const string SFX_VOLUME_KEY = "sfxVol";
    private const string PLAYER_MOVEMENT_KEY = "playerMove";

    // Constants
    private const int MIN_CHARACTERS = 3;
    private const int MAX_CHARACTERS = 16;
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;
    private const int MIN_MOVEMENT = 0;
    private const int MAX_MOVEMENT = 2;
    private const int MIN_SCORE = 0;
    private const int MAX_SCORE = 999999999;

    public static void CreateKeys() {
        // SetCurrentUserIndex(0);
        for (int i = 0; i < 5; i++) {
            USER_ACCOUNT_KEYS.Add("user" + i);
        }
    }

    public static bool AttemptToAddUsername(string username) {
        if (CheckForUsernameExistence(username)) {
            Debug.LogError("Username is already taken, unable to add.");
            return false;
        }
        if (username.Length >= MIN_CHARACTERS && username.Length <= MAX_CHARACTERS) {
            Debug.Log("Username added: " + username);
            int index = GetUserAccounts().Count;
            PlayerPrefs.SetString(USER_ACCOUNT_KEYS[index], username.ToUpper());
            PlayerPrefs.SetInt(CURRENT_USER_KEY, index);
            return true;
        } else {
            Debug.LogError("Username length must be in Range[" + MIN_CHARACTERS + ", " + MAX_CHARACTERS + "]: " + username);
            return false;
        }
    }

    public static void SetCurrentUserIndex(int index) {
        PlayerPrefs.SetInt(CURRENT_USER_KEY, index);
    }

    public static void SetMusicVolume(float volume) {
        if (volume >= MIN_VOLUME && volume <= MAX_VOLUME) {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
            Debug.Log("Music Volume set to: " + volume);
        } else {
            Debug.LogError("Music Volume must be in Range[" + Mathf.FloorToInt(MIN_VOLUME) + ", " + Mathf.FloorToInt(MAX_VOLUME) + "]: " + volume);
        }
    }

    public static void SetSfxVolume(float volume) {
        if (volume >= MIN_VOLUME && volume <= MAX_VOLUME) {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
            Debug.Log("SFX Volume set to: " + volume);
        } else {
            Debug.LogError("SFX Volume must be in Range[" + Mathf.FloorToInt(MIN_VOLUME) + ", " + Mathf.FloorToInt(MAX_VOLUME) + "]: " + volume);
        }
    }

    public static void SetPlayerMovement(int movement) {
        if (movement >= MIN_MOVEMENT && movement <= MAX_MOVEMENT) {
            PlayerPrefs.SetInt(PLAYER_MOVEMENT_KEY, movement);
            Debug.Log("Player Movement set to: " + movement);
        } else {
            Debug.LogError("Player Movement must be in Range[" + MIN_MOVEMENT + ", " + MAX_MOVEMENT + "]: " + movement);
        }
    }

    public static void AttemptToAddHighScore(int score) {
        if (GetUserAccounts().Count <= 0) {
            Debug.LogError("Can't add high scores without a profile");
            return;
        }
        if (score >= MIN_SCORE && score <= MAX_SCORE) {
            List<int> highScores = GetHighScores();
            highScores.Add(score);
            highScores.Sort((x, y) => y.CompareTo(x));
            if (highScores.Count > 10) {
                highScores.RemoveAt(highScores.Count - 1);
            }
            for (int i = 0; i < highScores.Count; i++) {
                PlayerPrefs.SetInt(GetHighScoreKey(GetCurrentUserAccount(), i), highScores[i]);
            }
            Debug.Log("High Score added: " + score);
        } else {
            Debug.LogError("High Score must be in Range[" + MIN_SCORE + ", " + MAX_SCORE + "]: " + score);
        }
    }

    public static List<string> GetUserAccounts() {
        List<string> temp = new List<string>();
        for (int i = 0; i < 5; i++) {
            if (HasUserAccount(i)) {
                temp.Add(PlayerPrefs.GetString(USER_ACCOUNT_KEYS[i]));
            }
        }
        return temp;
    }


    public static int GetCurrentUserIndex() {
        return PlayerPrefs.GetInt(CURRENT_USER_KEY);
    }

    public static string GetCurrentUserAccount() {
        if (GetUserAccounts().Count > 0) {
            return PlayerPrefs.GetString(USER_ACCOUNT_KEYS[GetCurrentUserIndex()]);
        } else {
            return "";
        }
    }

    public static string GetUserAccountByIndex(int index) {
        return PlayerPrefs.GetString(USER_ACCOUNT_KEYS[index]);
    }

    public static bool CheckForUsernameExistence(string username) {
        return GetUserAccounts().Contains(username.ToUpper());
    }

    public static bool DeleteUserAccount(int index) {
        if (HasUserAccount(index)) {
            DeleteUserAccountHighScores(index);
            PlayerPrefs.DeleteKey(USER_ACCOUNT_KEYS[index]);
            for (int i = index; i < 5; i++) {
                if (HasUserAccount(i + 1)) {
                    PlayerPrefs.SetString(USER_ACCOUNT_KEYS[i], GetUserAccountByIndex(i + 1));
                } else {
                    PlayerPrefs.DeleteKey(USER_ACCOUNT_KEYS[i]);
                    break;
                }
            }
            if (GetCurrentUserIndex() >= index) {
                SetCurrentUserIndex(index == 0 ? 0 : GetCurrentUserIndex() - 1);
            }
            return true;
        } else {
            Debug.Log("User account doesn't exist. Can't delete.");
            return false;
        }
    }

    private static void DeleteUserAccountHighScores(int index) {
        string username = GetUserAccountByIndex(index);
        for (int i = 0; i < 10; i++) {
            PlayerPrefs.DeleteKey(GetHighScoreKey(username, i));
        }
    }

    public static float GetMusicVolume() {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
    }

    public static float GetSfxVolume() {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
    }

    public static float GetPlayerMovement() {
        return PlayerPrefs.GetInt(PLAYER_MOVEMENT_KEY);
    }

    public static List<int> GetHighScores() {
        if (GetCurrentUserAccount() != "") {
            List<int> temp = new List<int>();
            for (int i = 0; i < 10; i++) {
                if (HasHighScore(i)) {
                    temp.Add(PlayerPrefs.GetInt(GetHighScoreKey(GetCurrentUserAccount(), i)));
                }
            }
            return temp;
        } else {
            return null;
        }
    }

    public static bool HasUserAccount(int index) {
        if (USER_ACCOUNT_KEYS.Count - index > 0) {
            return PlayerPrefs.HasKey(USER_ACCOUNT_KEYS[index]);
        }
        return false;
    }

    public static bool HasMusicVolume() {
        return PlayerPrefs.HasKey(MUSIC_VOLUME_KEY);
    }

    public static bool HasSfxVolume() {
        return PlayerPrefs.HasKey(SFX_VOLUME_KEY);
    }

    public static bool HasPlayerMovement() {
        return PlayerPrefs.HasKey(PLAYER_MOVEMENT_KEY);
    }

    public static bool HasHighScore(int index) {
        return PlayerPrefs.HasKey(GetHighScoreKey(GetCurrentUserAccount(), index));
    }

    private static string GetHighScoreKey(string username, int index) {
        return username + "highScore" + index;
    }
}