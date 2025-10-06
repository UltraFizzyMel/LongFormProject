using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Portals[] Portals { private set; get; }

    private void Awake()
    {
        Portals = GetComponentsInChildren<Portals>();

        if (Portals.Length != 2)
        {
            Debug.LogError("PortalPair must have exactly two Portals as children.");
        }
    }
}
