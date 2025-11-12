using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonmanager : MonoBehaviour
{
    public void REplay()
    {
        SceneManager.LoadScene("Rhett");
    }

    public void Quitting()
    {
        Application.Quit();
    }

    public void TUTLVL()
    {
        SceneManager.LoadScene("TUT_Scene");
    }

    public void LVL1()
    {
        SceneManager.LoadScene("Rhett");
        //SceneManager.LoadScene("Melissa");
    }

    public void LVL2()
    {
        SceneManager.LoadScene("LVL 2");
        //SceneManager.LoadScene("Melissa");
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void MEnu()
    {
        SceneManager.LoadScene("Main menu");
    }

    public void ReplayPreviousScene()
    {
        // Get the name of the scene that was active before loading the end screen
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");

        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found. Reloading default scene.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
}
