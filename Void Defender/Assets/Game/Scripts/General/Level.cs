using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {

    [SerializeField] float delay = 2f;

    public void LoadStartMenu() {
        SceneManager.LoadScene("Start Menu");
    }

    public void LoadGameManual1() {
        SceneManager.LoadScene("Game Manual 1");
    }

    public void LoadGameManual2() {
        SceneManager.LoadScene("Game Manual 2");
    }

    public void LoadGame() {
        if (FindObjectOfType<GameSession>()) {
            FindObjectOfType<GameSession>().ResetGame();
        }
        SceneManager.LoadScene("Game");
    }

    public void LoadGameOver() {
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad() {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game Over");
    }
}
