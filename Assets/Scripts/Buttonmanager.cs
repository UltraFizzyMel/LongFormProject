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
    }

    public void LVL2()
    {
        SceneManager.LoadScene("LVL 2");
    }

    public void MEnu()
    {
        SceneManager.LoadScene("Main menu");
    }
}
