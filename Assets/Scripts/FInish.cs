using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FInish : MonoBehaviour
{
    public GameObject WS;
    public PortalGun PG;
    public FirstPersonControls FPC;
    //public Portal PC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WS.SetActive(true);
            PG.enabled = false;
            FPC.enabled = false;
            //PC.enabled = false;
        }
    }
}
