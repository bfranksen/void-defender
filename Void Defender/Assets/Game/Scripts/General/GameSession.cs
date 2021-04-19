using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour {

    private static GameSession _instance = null;
    public static GameSession Instance {
        get { return _instance; }
    }

    private int health = 200;
    private int lives = 2;
    private int score = 0;

    // Start is called before the first frame update
    private void Awake() {
        SetUpSingleton();
        ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
        request.completed += Request_completed;
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

    public void ResetGame() {
        health = 200;
        lives = 2;
        score = 0;
    }

    public int Health { get => health; set => health = value; }
    public int Lives { get => lives; set => lives = value; }
    public int Score { get => score; set => score = value; }

    private void Request_completed(AsyncOperation obj) {
        BuildScriptableObject buildScriptableObject = ((ResourceRequest)obj).asset as BuildScriptableObject;

        if (buildScriptableObject == null) {
            Debug.LogError("Build scriptable object not found in resources directory! Check build log for errors!");
        } else {
            Debug.Log($"Build: {Application.version}.{buildScriptableObject.BuildNumber}");
        }
    }
}
