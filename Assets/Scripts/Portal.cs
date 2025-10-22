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
             Quaternion entryRotation = transform.rotation;

            // Call the other portal's teleport method and pass the incoming velocity + entry rotation
            otherPortal.MovePlayerToThisPortal(incoming, entryRotation); //(incoming, entryRotation);

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
                if (!foundOtherPortal && (portalObj != this.gameObject))
                {
                    otherPortalGO = portalObj;
                    otherPortal = otherPortalGO.GetComponent<Portal>();
                    foundOtherPortal = true; // Set flag to true after finding it
                   // Debug.Log("Player Velocity entering: " + firstPersonControls.velocity.magnitude);
                }
            }
        }
    }

    public void MovePlayerToThisPortal(Vector3 incomingVelocity,Quaternion entryPortalRotation)
    {
        
        
        otherPortal.disableTimer = 1f;    
        disableTimer = 1f;


       
        // --- CORRECT VELOCITY TRANSFORMATION ---
        // Get the portal normals
        Vector3 entryNormal = otherPortal.portalNormal; // The portal we entered through
        Vector3 exitNormal = this.portalNormal;         // The portal we're exiting through

        // Transform velocity from entry space to exit space
        Vector3 exitVelocity = TransformVelocityBetweenPortals(incomingVelocity, entryNormal, exitNormal);

        // --- SAFE TELEPORT OFFSET (avoid collider overlap) ---
        //SphereCollider portalCollider = GetComponent<SphereCollider>();
        // float portalRadius = portalCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        // float playerRadius = cc.radius;
        // float safetyBuffer = 0.6f;
        // float totalOffset = portalRadius + playerRadius + safetyBuffer;

        // Vector3 exitVelocity = portalNormal * firstPersonControls.velocity.magnitude;

        // Use forward of portal as its facing direction. Make sure when you place portals you set their rotation to face out of the surface.
        Vector3 exitPosition = transform.position + portalNormal * 2;
        

        Debug.DrawLine(transform.position, exitPosition, Color.red, 5f);
       
        Debug.DrawRay(exitPosition, Vector3.up * 0.5f, Color.green, 5f);

        // --- TELEPORT (disable CC while moving) ---
        cc.enabled = false;
        player.transform.position = exitPosition;

        // Update FirstPersonControls' previous position so trueVelocity isn't corrupted next frame
        //firstPersonControls.OverwritePreviousPosition(player.transform.position);

        cc.enabled = true;

        // Apply exit velocity properly to both velocity and portal velocity
        firstPersonControls.AddPortalExitVelocity(exitVelocity);
        //firstPersonControls.velocity = exitVelocity; // ensures gravity/system uses the new velocity

        Debug.Log("Player Velocity exiting: " + exitVelocity.magnitude);


    }

    private Vector3 TransformVelocityBetweenPortals(Vector3 incomingVel, Vector3 sourceNormal, Vector3 destNormal)
    {
        bool sourceIsVertical = IsVerticalPortal(sourceNormal);
        bool sourceIsHorizontal = IsHorizontalPortal(sourceNormal);
        bool destIsVertical = IsVerticalPortal(destNormal);
        bool destIsHorizontal = IsHorizontalPortal(destNormal);

        // CASE 1: Horizontal → Vertical (floor→wall) - FALLING MOMENTUM
        if (sourceIsHorizontal && destIsVertical && incomingVel.y < -2f)
        {
            return TransformHorizontalToVerticalWithMomentum(incomingVel, sourceNormal, destNormal);
        }

        // CASE 2: Vertical → Horizontal (wall→floor) - LAUNCHING TO FLOOR
        if (sourceIsVertical && destIsHorizontal)
        {
            return TransformVerticalToHorizontal(incomingVel, sourceNormal, destNormal);
        }

        // CASE 3: Horizontal → Horizontal (floor→floor or floor→ceiling)
        if (sourceIsHorizontal && destIsHorizontal)
        {
            return TransformHorizontalToHorizontal(incomingVel, sourceNormal, destNormal);
        }

        // CASE 4: Vertical → Vertical (wall→wall) 
        if (sourceIsVertical && destIsVertical)
        {
            return TransformVerticalToVertical(incomingVel, sourceNormal, destNormal);
        }

        // Fallback: simple rotation
        Quaternion rotation = Quaternion.FromToRotation(sourceNormal, -destNormal);
        return rotation * incomingVel;
    }



    private Vector3 TransformHorizontalToVerticalWithMomentum(Vector3 incomingVel, Vector3 sourceNormal, Vector3 destNormal)
    {
        // FLOOR → WALL: Convert falling momentum to horizontal launch from wall
        float fallingSpeed = Mathf.Abs(incomingVel.y);
        float launchSpeed = Mathf.Clamp(fallingSpeed * 1.2f, 8f, 30f);

        Vector3 wallForward = GetWallForwardDirection(destNormal);
        Vector3 launchDirection = (wallForward + (Vector3.up * 0.2f)).normalized;

        Debug.Log($"FLOOR→WALL: Falling speed {fallingSpeed} -> Launch {launchSpeed}");
        return launchDirection * launchSpeed;
    }

    

    private Vector3 TransformVerticalToHorizontal(Vector3 incomingVel, Vector3 sourceNormal, Vector3 destNormal)
    {
        // When going from wall to floor:
        // - Preserve horizontal momentum (XZ plane)
        // - Convert vertical momentum to forward momentum on the floor

        Vector3 horizontalVel = new Vector3(incomingVel.x, 0, incomingVel.z);
        float horizontalSpeed = horizontalVel.magnitude;

        // Use vertical speed as additional forward momentum
        float verticalSpeed = Mathf.Abs(incomingVel.y);
        float totalSpeed = Mathf.Max(horizontalSpeed + verticalSpeed * 0.5f, 2f);

        // Move in the direction of the floor portal's "forward" (use right if forward is up/down)
        Vector3 moveDirection = GetHorizontalDirection(destNormal);
        return moveDirection * totalSpeed;
    }

    private Vector3 TransformHorizontalToHorizontal(Vector3 incomingVel, Vector3 sourceNormal, Vector3 destNormal)
    {
        // FLOOR→FLOOR or FLOOR→CEILING: Simple rotation preserving vertical component
        Quaternion rotation = Quaternion.FromToRotation(sourceNormal, -destNormal);
        return rotation * incomingVel;
    }

    private Vector3 TransformVerticalToVertical(Vector3 incomingVel, Vector3 sourceNormal, Vector3 destNormal)
    {
        // WALL→WALL: Rotate horizontal velocity between wall orientations
        Quaternion rotation = Quaternion.FromToRotation(sourceNormal, -destNormal);
        return rotation * incomingVel;
    }

    private bool IsVerticalPortal(Vector3 normal)
    {
        return Mathf.Abs(normal.y) < 0.7f;
    }

    private bool IsHorizontalPortal(Vector3 normal)
    {
        return Mathf.Abs(normal.y) > 0.7f;
    }

    private Vector3 GetHorizontalDirection(Vector3 portalNormal)
    {
        // For horizontal portals (floors/ceilings), get a sensible forward direction
        if (Mathf.Abs(portalNormal.y) > 0.7f) // Floor or ceiling
        {
            // Use the portal's transform forward, but make sure it's horizontal
            Vector3 forward = transform.forward;
            forward.y = 0;
            return forward.normalized;
        }
        return portalNormal;
    }

    private Vector3 GetWallForwardDirection(Vector3 wallNormal)
    {
        // Use the portal's actual forward direction from its transform
        // This respects how you placed/rotated the portal
        Vector3 forward = transform.forward;

        // Make sure it's horizontal (parallel to the ground)
        forward.y = 0;

        // Normalize and return
        return forward.normalized;
    }

  

    public void MovePortal(RaycastHit raycastHit)
    {
        //Instantiate(this.gameObject, raycastHit.point, Quaternion.identity);
        hasMoved = true;
        transform.position = raycastHit.point;
        portalNormal = raycastHit.normal;
        Debug.DrawLine(raycastHit.point, raycastHit.normal, Color.green, 3f);
       

        transform.rotation = Quaternion.LookRotation(raycastHit.normal);
    }

}
