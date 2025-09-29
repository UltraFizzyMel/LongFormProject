using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    public GameObject RespawnPoint;

    public GameObject PLayer;

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
        if (other.gameObject.CompareTag ("Player"))
        {
            Debug.Log("Dead");
            PLayer.transform.position = RespawnPoint.transform.position;
        }
    }
}
