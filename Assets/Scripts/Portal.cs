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
            otherPortal.MovePlayerToThisPortal();
            Debug.Log("Player Velocity entering: " + cc.velocity.magnitude);
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

    public void MovePlayerToThisPortal()
    {
        
        
        otherPortal.disableTimer = 1f;    
        disableTimer = 1f;


        // 1. STORE the velocity FIRST, before any changes
        // Store both the original velocity and calculate exit velocity
        Vector3 incomingVelocity = cc.velocity;
        Vector3 exitVelocity = portalNormal * incomingVelocity.magnitude;

       
        




        // 2. Get the RADIUS of the PORTAL's Sphere Collider
        SphereCollider portalCollider = GetComponent<SphereCollider>();
        float portalRadius = portalCollider.radius;
        // Note: Remember to account for scale! If your portal is scaled, multiply by the scale.
        portalRadius *= Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);


        float playerRadius = cc.radius;
        float safetyBuffer = 1f; // A small extra distance to be safe

        // 3. Calculate the TOTAL offset needed.
        // This ensures the player's entire collider is clear of the portal's trigger.
        float totalOffset = portalRadius + playerRadius + safetyBuffer;

        // 4. Calculate the new spawn point using the total offset
        
        //Vector3 exitPosition = ExitPosition.transform.position;
        Vector3 exitPosition = transform.position + (portalNormal * totalOffset);

        
        

        Debug.DrawLine(transform.position, exitPosition, Color.red, 5f);
        Debug.DrawRay(exitPosition, Vector3.up * 0.5f, Color.green, 5f);

       
        cc.enabled = false;

        player.transform.position = exitPosition;
        
        cc.enabled = true;

        firstPersonControls.AddPortalExitVelocity(exitVelocity);


        Debug.Log("Player Velocity exiting: " + firstPersonControls.velocity);


    }

    public void MovePortal(RaycastHit raycastHit)
    {
        Instantiate(this.gameObject, raycastHit.point, Quaternion.identity);
        hasMoved = true;
       // transform.position = raycastHit.point;
        portalNormal = raycastHit.normal;
    }
}
