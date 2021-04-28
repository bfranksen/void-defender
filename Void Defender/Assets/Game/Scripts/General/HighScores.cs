using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighScores : MonoBehaviour {
    public const string privateCode = "AIGI2WdbL0a6UD5EIBV6pgU6aFYw3dykqFCoE4z-nkiQ";
    public const string publicCode = "608951ae8f40bb122830a236";
    public const string webURL = "http://dreamlo.com/lb/";

    public List<Highscore> highscoresList;
    GlobalHighScoresDisplay highScoresDisplay;
    public bool usernameExists = false;

    private void Awake() {
        highscoresList = new List<Highscore>();
        highScoresDisplay = GetComponent<GlobalHighScoresDisplay>();
    }

    public void AddNewHighscore(string username, int score) {
        StartCoroutine(UploadNewHighscore(username, score));
    }

    private IEnumerator UploadNewHighscore(string username, int score) {
        string uri = webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username.ToUpper()) + "/" + score;
        using (UnityWebRequest request = UnityWebRequest.Get(uri)) {
            yield return request.SendWebRequest();

            if (request.isNetworkError) { // Error
                Debug.Log(request.error);
            } else { // Success
                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    public void GetHighScores() {
        StartCoroutine(DownloadHighScores());
    }

    private IEnumerator DownloadHighScores() {
        string uri = webURL + publicCode + "/pipe/";
        using (UnityWebRequest request = UnityWebRequest.Get(uri)) {
            yield return request.SendWebRequest();

            if (request.isNetworkError) { // Error
                Debug.Log(request.error);
            } else { // Success
                FormatHighScores(request.downloadHandler.text);
                highScoresDisplay.OnHighscoresDownloaded(highscoresList);
                highScoresDisplay.CreateHighscoreRows();
            }
        }
    }

    private void FormatHighScores(string textStream) {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < entries.Length; i++) {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList.Add(new Highscore(i + 1, username, score));
            // print(highscoresList[i].place + ". " + highscoresList[i].username + " - " + highscoresList[i].score);
        }
    }

    public IEnumerator CheckUsernameExists(string username) {
        string uri = webURL + publicCode + "/pipe-get/" + UnityWebRequest.EscapeURL(username.ToUpper());
        print(uri);
        using (UnityWebRequest request = UnityWebRequest.Get(uri)) {
            yield return request.SendWebRequest();

            if (request.isNetworkError) { // Error
                Debug.LogError("Network Error: " + request.error);
            } else { // Success
                usernameExists = request.downloadHandler.text.Length > 0;
                Debug.Log("Valid request. " + usernameExists);
            }
        }
    }
}

public struct Highscore {
    public int place;
    public string username;
    public int score;

    public Highscore(int _place, string _username, int _score) {
        place = _place;
        username = _username;
        score = _score;
    }
}
