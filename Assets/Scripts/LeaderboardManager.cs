using UnityEngine;
using System.Collections.Generic;

public static class LeaderboardManager
{
    public static void SaveLeaderboard(string levelName, List<LeaderboardEntry> entries)
    {
        string json = JsonUtility.ToJson(new LeaderboardData { entries = entries });
        PlayerPrefs.SetString(levelName + "_Leaderboard", json);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard saved for level: " + levelName);
    }

    public static List<LeaderboardEntry> LoadLeaderboard(string levelName)
    {
        string key = levelName + "_Leaderboard";
        if (!PlayerPrefs.HasKey(key))
            return new List<LeaderboardEntry>();

        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<LeaderboardData>(json).entries;
    }
}
