using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawns : MonoBehaviour
{
    public Transform player;
    public Transform respawn;
    private CharacterController cc;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("playerdeath");
            cc = other.gameObject.GetComponent<CharacterController>();
            cc.enabled = false;
            player.position = respawn.position;
            cc.enabled = true;
        }
    }
}
