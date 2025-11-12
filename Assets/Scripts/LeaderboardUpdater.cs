using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardUpdater : MonoBehaviour
{
    public string levelName; // e.g., "Level1" or "Level2"
    public float playerTime; // Set this when the player finishes the level

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
    }
}
