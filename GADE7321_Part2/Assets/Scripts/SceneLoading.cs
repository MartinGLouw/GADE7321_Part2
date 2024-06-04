using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneLoading : MonoBehaviour
{
    public void LoadPVP()
    {
        SceneManager.LoadScene("PVPScene");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadEasy()
    {
        SceneManager.LoadScene("SPEasy");
    }

    public void LoadHard()
    {
        SceneManager.LoadScene("SPHard");
    }
}