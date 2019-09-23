using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject GameModeMenu;

    public void Start() {
        MainMenu.SetActive(true);
        GameModeMenu.SetActive(false);
    }

    public void PlayTutorial() {
        SceneManager.LoadScene("Tutorial");
    }

    public void PlayMultiplayer() {
        SceneManager.LoadScene("Battle");
    }

    public void ShowMenuPanel () {
        MainMenu.SetActive(true);
        GameModeMenu.SetActive(false);
    }

    public void ShowGameModePanel() {
        MainMenu.SetActive(false);
        GameModeMenu.SetActive(true);
    }

    public void QuitGame() {
        Debug.Log("Quit");
        Application.Quit();
    }
}
