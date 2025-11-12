using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*public class PlayerInfo
{
    public string playerName;
    public int score;
    public PlayerInfo(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
}*/

public class Leaderboard : MonoBehaviour
{
    //public string levelName;
    public string currentLevel;

    public GameObject Tab1;
    public Transform entryContainer1;
    public Transform entryTemplate1;

    public GameObject Tab2;
    public Transform entryContainer2;
    public Transform entryTemplate2;

    public GameObject Tab3;
    public Transform entryContainer3;
    public Transform entryTemplate3;

    /*private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);

        DisplayLeaderboard();
    }

    // Start is called before the first frame update
    void DisplayLeaderboard()
    {
        List<LeaderboardEntry> entries = LeaderboardManager.LoadLeaderboard(levelName);

        float templateHeight = 20f;
        for (int i = 0; i < entries.Count; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;
                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }

            /*entryTransform.Find("txtPos").GetComponent<TMPro.TextMeshProUGUI>().text = rankString;

            int score = Random.Range(0, 10000);
            entryTransform.Find("txtTime").GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();

            string playerName = "AAA";

            entryTransform.Find("txtName").GetComponent<TMPro.TextMeshProUGUI>().text = playerName;*/

    /*entryTransform.Find("txtPos").GetComponent<TextMeshProUGUI>().text = rankString;
    entryTransform.Find("txtName").GetComponent<TextMeshProUGUI>().text = entries[i].playerName;
    entryTransform.Find("txtTime").GetComponent<TextMeshProUGUI>().text = FormatTime(entries[i].time);
}

string FormatTime(float time)
{
    int minutes = Mathf.FloorToInt(time / 60);
    float seconds = time % 60;
    int milliseconds = Mathf.FloorToInt((time * 100) % 100);
    return $"{minutes:00}:{seconds:00.00}";
}
}*/

    /*private void Awake()
    {
        entryTemplate1.gameObject.SetActive(false);
        entryTemplate2.gameObject.SetActive(false);
        entryTemplate3.gameObject.SetActive(false);

        //PopulateLeaderboard();
    }

    // Call this to load a leaderboard for a specific level
    public void ShowLeaderboard(string levelName)
    {
        currentLevel = levelName;

        // Clear existing entries
        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate)
                Destroy(child.gameObject);
        }

        List<LeaderboardEntry> entries = LeaderboardManager.LoadLeaderboard(levelName);

        float templateHeight = 20f;
        for (int i = 0; i < entries.Count; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                default: rankString = rank + "TH"; break;
                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }

            entryTransform.Find("txtPos").GetComponent<TextMeshProUGUI>().text = rankString;
            entryTransform.Find("txtName").GetComponent<TextMeshProUGUI>().text = entries[i].playerName;
            entryTransform.Find("txtTime").GetComponent<TextMeshProUGUI>().text = FormatTime(entries[i].time);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }*/

    private void Awake()
    {
        // Ensure templates are inactive
        entryTemplate1.gameObject.SetActive(false);
        entryTemplate2.gameObject.SetActive(false);
        entryTemplate3.gameObject.SetActive(false);

        // Populate first tab by default
        PopulateLeaderboard("Level1", entryContainer1, entryTemplate1);
    }

    /// <summary>
    /// Clears old entries and populates a tab with the leaderboard for the specified level
    /// </summary>
    public void PopulateLeaderboard(string levelName, Transform container, Transform template)
    {
        // Clear old entries
        foreach (Transform child in container)
        {
            if (child != template)
                Destroy(child.gameObject);
        }

        // Load leaderboard for this level
        List<LeaderboardEntry> entries = LeaderboardManager.LoadLeaderboard(levelName);

        float templateHeight = 30f; // adjust spacing
        for (int i = 0; i < entries.Count; i++)
        {
            Transform entryTransform = Instantiate(template, container);
            entryTransform.gameObject.SetActive(true);

            RectTransform rect = entryTransform.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -templateHeight * i);

            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
                default: rankString = rank + "TH"; break;
            }

            entryTransform.Find("txtPos").GetComponent<TextMeshProUGUI>().text = rankString;
            entryTransform.Find("txtName").GetComponent<TextMeshProUGUI>().text = entries[i].playerName;
            entryTransform.Find("txtTime").GetComponent<TextMeshProUGUI>().text = FormatTime(entries[i].time);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes:00}:{seconds:00.00}";
    }

    public void ShowLevel1()
    {
        Tab1.SetActive(true);
        Tab2.SetActive(false);
        Tab3.SetActive(false);
        PopulateLeaderboard("Level1", entryContainer1, entryTemplate1);
    }

    public void ShowLevel2()
    {
        Tab1.SetActive(false);
        Tab2.SetActive(true);
        Tab3.SetActive(false);
        PopulateLeaderboard("Level2", entryContainer2, entryTemplate2);
    }

    public void ShowLevel3()
    {
        Tab1.SetActive(false);
        Tab2.SetActive(false);
        Tab3.SetActive(true);
        PopulateLeaderboard("Level3", entryContainer3, entryTemplate3);
    }
}
