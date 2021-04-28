using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPrefsController {

    // Keys
    private static List<string> USER_ACCOUNT_KEYS = new List<string>();
    private const string MUSIC_VOLUME_KEY = "musicVol";
    private const string SFX_VOLUME_KEY = "sfxVol";
    private const string PLAYER_MOVEMENT_KEY = "playerMove";
    private static List<string> HIGH_SCORE_KEYS = new List<string>();

    // Constants
    private const int MIN_CHARACTERS = 3;
    private const int MAX_CHARACTERS = 16;
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;
    private const int MIN_MOVEMENT = 0;
    private const int MAX_MOVEMENT = 2;
    private const int MIN_SCORE = 0;
    private const int MAX_SCORE = 999999999;
    public static int currentAccountIndex;

    public static void CreateKeys() {
        for (int i = 0; i < 5; i++) {
            USER_ACCOUNT_KEYS.Add("user" + i);
        }
        for (int i = 0; i < 10; i++) {
            HIGH_SCORE_KEYS.Add("highScore" + i);
        }
    }

    public static bool AttemptToAddUsername(string username) {
        if (CheckForUsernameExistence(username.ToUpper())) {
            Debug.LogError("Username is already taken, unable to add.");
            return false;
        }
        if (username.Length >= MIN_CHARACTERS && username.Length <= MAX_CHARACTERS) {
            Debug.Log("Username added: " + username);
            PlayerPrefs.SetString(USER_ACCOUNT_KEYS[GetUserAccounts().Count], username.ToUpper());
            return true;
        } else {
            Debug.LogError("Username length must be in Range[" + MIN_CHARACTERS + ", " + MAX_CHARACTERS + "]: " + username);
            return false;
        }
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
        if (score >= MIN_SCORE && score <= MAX_SCORE) {
            Debug.Log("High Score added: " + score);
        } else {
            Debug.LogError("High Score must be in Range[" + MIN_SCORE + ", " + MAX_SCORE + "]: " + score);
        }

        List<int> highScores = GetHighScores();
        highScores.Add(score);
        highScores.Sort((x, y) => y.CompareTo(x));
        if (highScores.Count > 10) {
            highScores.RemoveAt(highScores.Count - 1);
        }
        for (int i = 0; i < highScores.Count; i++) {
            PlayerPrefs.SetInt(HIGH_SCORE_KEYS[i], highScores[i]);
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

    public static bool CheckForUsernameExistence(string username) {
        return GetUserAccounts().Contains(username);
    }

    public static void SetCurrentUserAccount(int index) {
        currentAccountIndex = index;
    }

    public static string GetCurrentUserAccount() {
        return PlayerPrefs.GetString(USER_ACCOUNT_KEYS[currentAccountIndex]);
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
        List<int> temp = new List<int>();
        for (int i = 0; i < 10; i++) {
            if (HasHighScore(i)) {
                temp.Add(PlayerPrefs.GetInt(HIGH_SCORE_KEYS[i]));
            }
        }
        return temp;
    }

    public static bool HasUserAccount(int index) {
        return PlayerPrefs.HasKey(USER_ACCOUNT_KEYS[index]);
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
        return PlayerPrefs.HasKey(HIGH_SCORE_KEYS[index]);
    }
}
