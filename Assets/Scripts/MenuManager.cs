using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartEditor()
    {

       SceneManager.LoadScene("Editor");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
