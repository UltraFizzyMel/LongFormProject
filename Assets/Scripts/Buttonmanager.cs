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
}
