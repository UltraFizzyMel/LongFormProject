using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public Leaderboard leaderboard;
    public GameObject Tab1;
    public GameObject Tab2;
    public GameObject Tab3;

    public Button button1;
    public Button button2;
    public Button button3;

    Color c1;
    Color c2;
    Color c3;

    public void Start()
    {
        c1 = button1.image.color;
        c2 = button2.image.color;
        c3 = button3.image.color;

        TabOne();
    }

    public void TabOne()
    {
        Tab2.SetActive(false);
        c2.a = 0.3f;
        button2.image.color = c2;

        Tab3.SetActive(false);
        c3.a = 0.3f;
        button3.image.color = c3;

        c1.a = 1f;
        button1.image.color = c1;
        Tab1.SetActive(true);

        //leaderboard.ShowLeaderboard("Level1");
        leaderboard.ShowLevel1();
    }

    public void TabTwo()
    {
        Tab1.SetActive(false);
        c1.a = 0.3f;
        button1.image.color = c1;

        Tab3.SetActive(false);
        c3.a = 0.3f;
        button3.image.color = c3;

        Tab2.SetActive(true);
        c2.a = 1f;
        button2.image.color = c2;

        //leaderboard.ShowLeaderboard("Level2");
        leaderboard.ShowLevel2();
    }

    public void TabThree()
    {
        Tab1.SetActive(false);
        c1.a = 0.3f;
        button1.image.color = c1;

        Tab2.SetActive(false);
        c2.a = 0.3f;
        button2.image.color = c2;

        Tab3.SetActive(true);
        c3.a = 1f;
        button3.image.color = c3;

        //leaderboard.ShowLeaderboard("Level3");
        leaderboard.ShowLevel3();
    }
}
