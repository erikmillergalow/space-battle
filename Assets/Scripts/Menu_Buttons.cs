using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Buttons : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject GameModeMenu;
    // Start is called before the first frame update
    public void Start()
    {
        MainMenu.SetActive(true);
        GameModeMenu.SetActive(false);
    }
    public void PlayFreePlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayMultiplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void ShowMenuPanel ()
    {
        MainMenu.SetActive(true);
        GameModeMenu.SetActive(false);
    }
    public void ShowGameModePanel()
    {
        MainMenu.SetActive(false);
        GameModeMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
