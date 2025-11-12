using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject Tab1;
    public GameObject Tab2;
    public Button button1;
    public Button button2;
    Color c1;
    Color c2;

    public void Start()
    {
        c1 = button1.image.color;
        c2 = button2.image.color;
    }

    public void TabOne()
    {
        Tab2.SetActive(false);
        c2.a = 0.3f;
        button2.image.color = c2;

        c1.a = 1f;
        button1.image.color = c1;
        Tab1.SetActive(true);
    }

    public void TabTwo()
    {
        Tab1.SetActive(false);
        c1.a = 0.3f;
        button1.image.color = c1;

        Tab2.SetActive(true);
        c2.a = 1f;
        button2.image.color = c2;
    }
}
