using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private string LvlSc;
    [SerializeField]
    private string PracLvl;

    public void Replay ()
    {
        SceneManager.LoadScene("Rhett");
    }

    public void Quiting()
    {
        Application.Quit();
    }

    public void lvlone ()
    {
        SceneManager.LoadScene(LvlSc);
    }

    public void Prac()
    {
        SceneManager.LoadScene(PracLvl);
    }
}
