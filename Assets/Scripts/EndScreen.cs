using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI messageText; // Add another Text UI for the personalized message

    void Start()
    {
        // Retrieve saved data
        string result = PlayerPrefs.GetString("Result", "Lose");
        float elapsedTime = PlayerPrefs.GetFloat("Time", 0f); // total time in seconds

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int hundredths = Mathf.FloorToInt((elapsedTime * 100) % 100);

        // Show the basic info
        //resultText.text = $"You got {result}!";
        if (result != "Lose")
            timeText.text += string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);

        // Set color and message based on performance
        switch (result)
        {
            case "Gold":
                resultText.color = new Color(1f, 0.84f, 0f); // gold-ish
                resultText.text = "Gold Achieved!";
                messageText.text = "Engines calibrated, shields stable, and coffee still hot.";
                break;

            case "Silver":
                resultText.color = Color.grey;
                resultText.text = "Silver earned!";
                messageText.text = "The ship's operational - if you don't mind a few sparks...";
                break;

            case "Bronze":
                resultText.color = new Color(0.8f, 0.5f, 0.2f); // bronze tone
                resultText.text = "Bronze secured!";
                messageText.text = "You've done enough to keep her alive. Barely.";
                break;

            default:
                resultText.color = Color.red;
                resultText.text = "Mission Failed.";
                messageText.text = "The clock hit zero, and the ship's still in pieces. Time to suit up and try again.";
                timeText.text += "Did not finish.";
                break;
        }
    }
}
