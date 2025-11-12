using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUpdater : MonoBehaviour
{
    /*private string levelName;
    private float playerTime;
    private float timeLimit;

    public bool IsNewHighScore(float newTime)
    {
        var entries = LeaderboardManager.LoadLeaderboard(levelName);

        if (entries.Count < 10)
            return true;

        return newTime < entries.Max(e => e.time);
    }

    public void TryAddNewTime(string playerName, float newTime)
    {
        var entries = LeaderboardManager.LoadLeaderboard(levelName);
        entries.Add(new LeaderboardEntry { playerName = playerName, time = newTime });

        entries = entries.OrderBy(e => e.time).Take(10).ToList();

        LeaderboardManager.SaveLeaderboard(levelName, entries);
    }*/

    public GameObject nameEntryPanel;        // UI panel for entering name (set in Inspector)
    public TMP_InputField nameInputField;    // Input field inside that panel

    private string levelName;
    private float playerTime;
    private float timeLimit;

    public Button btnRestart;
    public Button btnMainMenu;
    public Button btnQuit;
    public Button btnLeaderboard;

    private void Start()
    {
        // Retrieve the previous level’s info
        string tempName = PlayerPrefs.GetString("PreviousScene", "Level1");
        if (tempName == "Rhett")
            levelName = "Level1";
        else if (tempName == "LVL 2")
            levelName = "Level2";
        else if (tempName == "Level 3")
            levelName = "Level3";
        else
            levelName = tempName;

        //levelName = PlayerPrefs.GetString("PreviousScene", "Level1");
        playerTime = PlayerPrefs.GetFloat("Time", 0);
        timeLimit = PlayerPrefs.GetFloat("TimeLimit", 0);

        Debug.Log($"Loaded end screen for {levelName}, time = {playerTime}, limit = {timeLimit}");

        // Check if player completed within time limit
        if (playerTime > timeLimit)
            return;

        string result = PlayerPrefs.GetString("Result", "Lose");

        // Player finished within time limit — check for leaderboard qualification
        if (IsNewHighScore(playerTime) && result != "Lose")
        {
            btnRestart.interactable = false;
            btnMainMenu.interactable = false;
            btnQuit.interactable = false;
            btnLeaderboard.interactable = false;

            nameEntryPanel.SetActive(true); // prompt for name
        }
    }

    public bool IsNewHighScore(float newTime)
    {
        var entries = LeaderboardManager.LoadLeaderboard(levelName);
        if (entries.Count < 10)
            return true;
        return newTime < entries.Max(e => e.time);
    }

    public void SubmitName()
    {
        string playerName = nameInputField.text.ToUpper();
        if (playerName.Length > 3)
            playerName = playerName.Substring(0, 3);

        TryAddNewTime(playerName, playerTime);
        nameEntryPanel.SetActive(false);

        btnRestart.interactable = true;
        btnMainMenu.interactable = true;
        btnQuit.interactable = true;
        btnLeaderboard.interactable = true;

        //leaderboardPanel.SetActive(true);
    }

    public void TryAddNewTime(string playerName, float newTime)
    {
        var entries = LeaderboardManager.LoadLeaderboard(levelName);
        entries.Add(new LeaderboardEntry { playerName = playerName, time = newTime });

        // Sort ascending (best times first)
        entries = entries.OrderBy(e => e.time).Take(10).ToList();

        LeaderboardManager.SaveLeaderboard(levelName, entries);
    }
}
