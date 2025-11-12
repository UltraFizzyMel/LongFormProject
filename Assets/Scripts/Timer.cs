using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq.Expressions;

public class Timer : MonoBehaviour
{
    public NewFinish finish;

    [SerializeField] public float gameDuration = 120f;
    public bool gameEnded = false;

    [SerializeField] TextMeshProUGUI timerText;
    public float elapsedTime;

    public bool bronzeMedal = false;
    public bool silverMedal = false;
    public bool goldMedal = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded) return;

        elapsedTime+= Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        float goldTime = gameDuration / 3f;
        float silverTime = gameDuration * 2f / 3f;

        if (elapsedTime <= goldTime)
        {
            goldMedal = true;
            silverMedal = false;
            bronzeMedal = false;
        }
        else if (elapsedTime <= silverTime)
        {
            goldMedal = false;
            silverMedal = true;
            bronzeMedal = false;
        }
        else if (elapsedTime <= gameDuration)
        {
            goldMedal = false;
            silverMedal = false;
            bronzeMedal = true;
        }
        else if (elapsedTime >= gameDuration)
        {
            finish.EndGame("Lose");
        }
    }
}
