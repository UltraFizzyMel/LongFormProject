using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public GameObject Panel;
    public bool isFinish;

    public FirstPersonControls FPC;
    // Start is called before the first frame update
    void Start()
    {
        Panel.SetActive(false); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Time.timeScale = 1f;
        FPC.canPlayerMove = true;

    }

    // Update is called once per frame
    void Update()
    {
       if (isFinish == true)
        {
            Panel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //Time.timeScale = 0f;
            FPC.canPlayerMove = true;
        }
    }
}
