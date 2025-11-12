using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLeaderboard : MonoBehaviour
{
    public TabManager tabManager;
    public void Reset()
    {
        PlayerPrefs.DeleteKey("Level1_Leaderboard");
        PlayerPrefs.DeleteKey("Level2_Leaderboard");
        PlayerPrefs.DeleteKey("Level3_Leaderboard");
        PlayerPrefs.Save();
        tabManager.TabOne();
    }
}
