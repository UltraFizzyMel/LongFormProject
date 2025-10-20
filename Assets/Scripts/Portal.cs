using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject otherPortalGO;

    private Vector3 portalNormal;

    private Portal otherPortal;

    public GameObject player;
    //private PlayerMotor motor;
    private FirstPersonControls firstPersonControls;
    private CharacterController cc;
    public GameObject portalRed;
    public GameObject portalBlue;
   
    

    public bool hasMoved = false;

    float disableTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //motor = player.GetComponent<PlayerMotor>();
       // motor = GameObject.FindAnyObjectByType<PlayerMotor>();
        cc = player.GetComponent<CharacterController>();
        firstPersonControls = player.GetComponent<FirstPersonControls>();  
        //cc = GameObject.FindAnyObjectByType<CharacterController>();

        disableTimer = 0f;

        SetOtherPortal();
    }

    private void OnTriggerEnter(Collider collider)
    {
       //otherPortalGO.GetComponent<SphereCollider>().enabled = false;
        if (collider.gameObject == player && disableTimer <= 0 && hasMoved && otherPortal.hasMoved)
        {
            // Capture the incoming velocity and this portal's rotation BEFORE teleporting
            Vector3 incoming = firstPersonControls.velocity;
            // Quaternion entryRotation = transform.rotation;

            // Call the other portal's teleport method and pass the incoming velocity + entry rotation
            otherPortal.MovePlayerToThisPortal(); //(incoming, entryRotation);

            Debug.Log("Player Velocity entering: " + incoming.magnitude);

        } // currently checks if game object was a player or not 
    }

   /* private void OnTriggerExit2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }*/

    // Update is called once per frame
    void Update()
    {
        if (disableTimer > 0)
        {
            disableTimer -= Time.deltaTime; 
        }
    }

    private void SetOtherPortal()
    {
        if (otherPortal == null)
        {
            bool foundOtherPortal = false; // Flag to track if we found it
            GameObject[] allPortalObjects = GameObject.FindGameObjectsWithTag("Portal");

            foreach (var portalObj in allPortalObjects)
            {
                if (!foundOtherPortal && portalObj != this.gameObject)
                {
                    otherPortalGO = portalObj;
                    otherPortal = otherPortalGO.GetComponent<Portal>();
                    foundOtherPortal = true; // Set flag to true after finding it
                   // Debug.Log("Player Velocity entering: " + firstPersonControls.velocity.magnitude);
                }
            }
        }
    }

    public void MovePlayerToThisPortal() // (Vector3 incomingVelocity, Quaternion entryPortalRotation)
    {
        
        
        otherPortal.disableTimer = 1f;    
        disableTimer = 1f;


        // --- ROTATE THE VELOCITY from entry portal space to exit portal space ---
        // 'this.transform.rotation' is the exit portal rotation
       // Quaternion exitRotation = transform.rotation;
       // Quaternion rotationDifference = exitRotation * Quaternion.Inverse(entryPortalRotation);
       // Vector3 exitVelocity = rotationDifference * incomingVelocity;

        // --- SAFE TELEPORT OFFSET (avoid collider overlap) ---
        //SphereCollider portalCollider = GetComponent<SphereCollider>();
       // float portalRadius = portalCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
       // float playerRadius = cc.radius;
       // float safetyBuffer = 0.6f;
       // float totalOffset = portalRadius + playerRadius + safetyBuffer;

        Vector3 exitVelocity = portalNormal * firstPersonControls.velocity.magnitude;

        // Use forward of portal as its facing direction. Make sure when you place portals you set their rotation to face out of the surface.
        Vector3 exitPosition = transform.position + otherPortal.portalNormal * 2;
        

        Debug.DrawLine(transform.position, exitPosition, Color.red, 5f);
        Debug.DrawRay(exitPosition, Vector3.up * 0.5f, Color.green, 5f);

        // --- TELEPORT (disable CC while moving) ---
        cc.enabled = false;
        player.transform.position = exitPosition;

        // Update FirstPersonControls' previous position so trueVelocity isn't corrupted next frame
        //firstPersonControls.OverwritePreviousPosition(player.transform.position);

        cc.enabled = true;

        // Apply exit velocity properly to both velocity and portal velocity
        //firstPersonControls.AddPortalExitVelocity(exitVelocity);
        firstPersonControls.SetVelocity(exitVelocity); // ensures gravity/system uses the new velocity

        Debug.Log("Player Velocity exiting: " + exitVelocity.magnitude);


    }

    public void MovePortal(RaycastHit raycastHit)
    {
        //Instantiate(this.gameObject, raycastHit.point, Quaternion.identity);
        hasMoved = true;
        transform.position = raycastHit.point;
        portalNormal = raycastHit.normal;
        transform.rotation = Quaternion.LookRotation(raycastHit.normal);
    }
}
