using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CameraMove))]
public class PortalPlacement : MonoBehaviour
{
    [SerializeField]
    private PortalPair portals;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Crosshair crosshair;

    //private CameraMove cameraMove;

    private void Awake()
    {
        //cameraMove = GetComponent<CameraMove>();
    }
    void Update()
    {
        
    }
}
