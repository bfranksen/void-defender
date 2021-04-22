using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GameSession : MonoBehaviour {

    // INSTANCE
    private static GameSession _instance = null;
    public static GameSession Instance {
        get { return _instance; }
    }

    // GOOGLE PLAY
#if UNITY_ANDROID
    public static PlayGamesPlatform platform;
#endif

    // PLAYER INFO
    private int health = 200;
    private int lives = 2;
    private int score = 0;
    public int Health { get => health; set => health = value; }
    public int Lives { get => lives; set => lives = value; }
    public int Score { get => score; set => score = value; }

    private void Awake() {
        ActivatePlatformAndLogIn();
        SetUpSingleton();
    }

    private void SetUpSingleton() {
#if !UNITY_WEBGL
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;
#endif
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void ActivatePlatformAndLogIn() {
#if UNITY_ANDROID
        if (platform == null) {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            platform = PlayGamesPlatform.Activate();
        }

        Social.Active.localUser.Authenticate(success => {
            if (success) {
                Debug.Log("Logged in successfully!");
            } else {
                Debug.Log("Failed to log in!");
            }
        });
#endif
    }

    public void ResetGame() {
        health = 200;
        lives = 2;
        score = 0;
    }
}
