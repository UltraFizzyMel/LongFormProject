using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{

    public float gunRange = 1000f;
    public Portal[] portals;
    //[SerializeField] private InputManager inputManager;
    [SerializeField] private FirstPersonControls firstPersonControls;
    public LayerMask portalable;
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        portals = GameObject.FindObjectsOfType<Portal>();
        if (portals.Length != 2)
        {
            Debug.LogWarning("Expected 2 portals but found " + portals.Length); 
        }

        if (firstPersonControls != null)
        {
            firstPersonControls.OnPortalShot += ShootPortal;
        }

    }

    // Update is called once per frame
    public void ShootPortal(int portalIndex)
    {
       
        
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit rayCastHit;
        Debug.DrawRay(ray.origin, ray.direction * gunRange, Color.yellow, 1.0f);

        if (Physics.Raycast(ray, out rayCastHit, gunRange, portalable)) 
        {
            
            Debug.DrawRay(ray.origin, ray.direction * gunRange, Color.yellow, 1.0f);
            if (portalIndex >= 0 && portalIndex < portals.Length)
            {
                // 4. Command the specific portal (the 0th or the 1st in the array) to move.
                portals[portalIndex].MovePortal(rayCastHit);
                Debug.Log($"ShootPortal: Shot portal {portalIndex} at " + rayCastHit.point);
            }

        }
        else
        {
            Debug.LogError($"ShootPortal: Invalid portal index {portalIndex}!");
        }
    }

   


}
