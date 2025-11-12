using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewFinish : MonoBehaviour
{
    public Timer timer;
    public float timeLimit;
    public bool isDeath = false;

    // Start is called before the first frame update
    void Start()
    {
        timeLimit = timer.gameDuration;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        timer.gameEnded = true;

        if (isDeath)
        {
            EndGame("Lose");
            return;
        }

        if (timer.goldMedal == true)
            EndGame("Gold");
        else if (timer.silverMedal == true)
            EndGame("Silver");
        else if (timer.bronzeMedal == true)
            EndGame("Bronze");
        else
            EndGame("Lose");
    }

    public void EndGame(string result)
    {
        PlayerPrefs.SetString("Result", result);  // save medal type
        PlayerPrefs.SetFloat("Time", timer.elapsedTime); // save completion time
        PlayerPrefs.SetFloat("TimeLimit", timeLimit);
        PlayerPrefs.Save();

        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("End");
    }
}
