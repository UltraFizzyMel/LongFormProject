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
    public string levelName;
    public Transform entryContainer;
    public Transform entryTemplate;

    private void Awake()
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

            entryTransform.Find("txtPos").GetComponent<TextMeshProUGUI>().text = rankString;
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
    }
}
