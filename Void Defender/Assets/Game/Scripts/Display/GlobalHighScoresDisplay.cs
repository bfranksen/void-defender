using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalHighScoresDisplay : MonoBehaviour {

    [SerializeField] GameObject highscoreTemplate;
    List<GameObject> highscoreTexts;
    List<Highscore> highscoresList;
    Vector3 currentPos;
    Vector3 offsetVector;
    HighScores highscoreManager;
    int currentPage = 0;

    void Start() {
        highscoreTexts = new List<GameObject>();
        offsetVector = new Vector3(0f, 58f, 0f);
        currentPos = highscoreTemplate.transform.position;
        currentPos -= offsetVector;
        highscoreManager = GetComponent<HighScores>();
        StartCoroutine(RefreshHighScores());

        for (int i = 0; i < 10; i++) {
            GameObject row = Instantiate(highscoreTemplate, highscoreManager.gameObject.transform, true) as GameObject;
            row.transform.position = currentPos;
            row.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + ".";
            row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Fetching...";
            row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "-----";
            // row.transform.parent = highscoreManager.gameObject.transform;
            highscoreTexts.Add(row);
            currentPos -= offsetVector;
        }
        highscoreTemplate.SetActive(false);
    }

    public void OnHighscoresDownloaded(List<Highscore> highscoresList) {
        this.highscoresList = highscoresList;
    }

    public void CreateHighscoreRows() {
        List<Highscore> scoresToShow = GetScoresToShow(highscoresList);
        for (int i = 0; i < scoresToShow.Count; i++) {
            highscoreTexts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = scoresToShow[i].place.ToString() + ".";
            highscoreTexts[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = scoresToShow[i].username;
            highscoreTexts[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = scoresToShow[i].score.ToString();
        }
        if (scoresToShow.Count < 10) {
            for (int i = scoresToShow.Count; i < 10; i++) {
                highscoreTexts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (GetPageIndex() + i + 1) + ".";
                highscoreTexts[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "-----";
                highscoreTexts[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "-----";
            }
        }
    }

    private List<Highscore> GetScoresToShow(List<Highscore> highscoresList) {
        List<Highscore> scoresToShow = new List<Highscore>(highscoresList.GetRange(GetPageIndex(), Mathf.Min(highscoresList.Count - GetPageIndex(), 10)));
        // foreach (Highscore hs in scoresToShow) {
        //     print(hs.place + ". " + hs.username + " - " + hs.score);
        // }
        return scoresToShow;
    }

    public void ShowYourScorePage() {
        string username = PlayerPrefsController.GetCurrentUserAccount();
        if (username != "") {
            foreach (Highscore hs in highscoresList) {
                if (hs.username == username) {
                    currentPage = Mathf.FloorToInt(hs.place / 10f);
                    break;
                }
            }
        }
        CreateHighscoreRows();
    }

    public void ShowFirstPage() {
        currentPage = 0;
        CreateHighscoreRows();
    }

    public void ShowPreviousPage() {
        currentPage = Mathf.Max(0, currentPage - 1);
        CreateHighscoreRows();
    }

    public void ShowNextPage() {
        currentPage = Mathf.Min(GetNumberOfPages(), currentPage + 1);
        CreateHighscoreRows();
    }

    public void ShowLastPage() {
        currentPage = GetNumberOfPages();
        CreateHighscoreRows();
    }

    public int GetNumberOfPages() {
        return Mathf.FloorToInt(highscoresList.Count / 10f);
    }
    private int GetPageIndex() {
        return currentPage * 10;
    }

    IEnumerator RefreshHighScores() {
        while (true) {
            highscoreManager.GetHighScores();
            yield return new WaitForSeconds(30f);
        }
    }
}