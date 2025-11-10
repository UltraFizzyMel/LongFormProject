using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PortalPlayer : MonoBehaviour
{
    /*[Header("References")]
    public Rigidbody rb;
    public CharacterController characterController;

    [HideInInspector]
    public bool isWarping = false;

    private Portals inPortal;
    private Portals outPortal;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0f, 180f, 0f);

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!characterController) characterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var portal = other.GetComponent<Portals>();
        if (portal == null) return;

        // Only teleport if player is not already warping
        if (!isWarping && portal.isPlaced && portal.OtherPortal.isPlaced)
        {
            inPortal = portal;
            outPortal = portal.OtherPortal;
            Warp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var portal = other.GetComponent<Portals>();
        if (portal == null) return;

        // Reset warp state
        if (portal == inPortal)
        {
            inPortal = null;
            outPortal = null;
        }
    }

    public void Warp()
    {
        if (inPortal == null || outPortal == null) return;

        isWarping = true;

        Transform inT = inPortal.transform;
        Transform outT = outPortal.transform;

        // 1️⃣ Transfer velocity
        //Vector3 relativeVel = inT.InverseTransformDirection(rb.velocity);
        //relativeVel = halfTurn * relativeVel;
        //rb.velocity = outT.TransformDirection(relativeVel);

        // 2️⃣ Compute relative position
        Vector3 relativePos = inT.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;

        // 3️⃣ Teleport player
        characterController.enabled = false; // Disable CC to avoid collision issues
        transform.position = outT.TransformPoint(relativePos);

        // 4️⃣ Rotate player to match portal exit
        Quaternion relativeRot = Quaternion.Inverse(inT.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outT.rotation * relativeRot;

        characterController.enabled = true; // Re-enable CC

        StartCoroutine(ResetWarping());
    }

    private IEnumerator ResetWarping()
    {
        // Prevent multiple warps in a single frame
        yield return new WaitForEndOfFrame();
        isWarping = false;
    }

    private void Update()
    {
        if (inPortal != null && outPortal != null)
        {
            CheckAndWarp();
        }
    }

    public void SetPortals(Portals entry, Portals exit)
    {
        inPortal = entry;
        outPortal = exit;
    }

    public void ClearPortals(Portals entry, Portals exit)
    {
        if (inPortal == entry && outPortal == exit)
        {
            inPortal = null;
            outPortal = null;
        }
    }

    private void CheckAndWarp()
    {
        if (isWarping) return;

        // Determine if player has crossed the portal plane
        Vector3 toPortal = transform.position - inPortal.transform.position;
        float dot = Vector3.Dot(toPortal, inPortal.transform.forward);

        if (dot > 0f) // player has passed the portal
        {
            Warp();
        }
    }*/

    private Portals inPortal = null;
    private Portals outPortal = null;
    private CharacterController cc;
    private FirstPersonControls fpc;
    private bool isInPortalZone = false;

    // Track last frame position relative to the portal plane for crossing detection
    private Vector3 lastLocalPosInPortalSpace = Vector3.zero;
    private bool hadLastLocalPos = false;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        fpc = GetComponent<FirstPersonControls>();
        if (fpc == null) Debug.LogWarning("PortalPlayer: FirstPersonControls not found on player.");
    }

    private void Update()
    {
        if (!isInPortalZone || inPortal == null || outPortal == null)
        {
            hadLastLocalPos = false;
            return;
        }

        // Convert world position to portal local space to check sign of z (front/back)
        Vector3 localPos = inPortal.transform.InverseTransformPoint(transform.position);

        if (!hadLastLocalPos)
        {
            lastLocalPosInPortalSpace = localPos;
            hadLastLocalPos = true;
            return;
        }

        // if player moved from positive z to negative z in portal local space => crossed plane
        // (Z > 0 is in front of portal plane in this setup)
        if (lastLocalPosInPortalSpace.z > 0f && localPos.z <= 0f)
        {
            // Crossing detected — perform teleport
            TeleportThroughPortal();
            // Reset lastLocalPos so we don't immediately re-teleport
            hadLastLocalPos = false;
            return;
        }

        lastLocalPosInPortalSpace = localPos;
    }

    private void TeleportThroughPortal()
    {
        Debug.Log($"Teleporting through {inPortal.name} → {outPortal.name}");
        if (inPortal == null || outPortal == null) return;

        // Compute new position & rotation relative to the exit portal
        // Step 1: position relative to inPortal
        Vector3 localPosition = inPortal.transform.InverseTransformPoint(transform.position);
        // rotate 180 around Y to match Portal behaviour
        localPosition = Quaternion.Euler(0f, 180f, 0f) * localPosition;
        Vector3 newWorldPos = outPortal.transform.TransformPoint(localPosition);

        // Step 2: rotation
        Quaternion localRot = Quaternion.Inverse(inPortal.transform.rotation) * transform.rotation;
        localRot = Quaternion.Euler(0f, 180f, 0f) * localRot;
        Quaternion newWorldRot = outPortal.transform.rotation * localRot;

        // Step 3: velocity transformation (try to preserve player's world velocity)
        Vector3 incomingVelocity = Vector3.zero;
        if (fpc != null)
        {
            incomingVelocity = fpc.velocity; // uses your public velocity vector
        }
        // Transform velocity into inPortal local space, rotate 180 on Y, then transform out
        Vector3 localVel = inPortal.transform.InverseTransformDirection(incomingVelocity);
        localVel = Quaternion.Euler(0f, 180f, 0f) * localVel;
        Vector3 newWorldVel = outPortal.transform.TransformDirection(localVel);

        // Step 4: safely move character controller (disable/enable to avoid collisions)
        StartCoroutine(DoSafeTeleport(newWorldPos, newWorldRot, newWorldVel));
    }

    private IEnumerator DoSafeTeleport(Vector3 position, Quaternion rotation, Vector3 exitVelocity)
    {
        // Optionally fade or freeze input here
        // Disable character controller to avoid collision issues with immediate repositioning
        cc.enabled = false;

        // apply position & rotation
        transform.position = position;
        // If you have a separate camera or first-person root that handles rotation, set it appropriately.
        // Here we set the player's yaw (transform) and also rotate the camera pitch if possible.
        transform.rotation = Quaternion.Euler(new Vector3(0f, rotation.eulerAngles.y, 0f));

        // If the player camera pitch is handled in FirstPersonControls, set that local rotation
        if (fpc != null && fpc.playerCamera != null)
        {
            // preserve camera pitch: compute camera's local pitch from full rotation
            float pitch = fpc.playerCamera.localEulerAngles.x;
            // set camera global rotation to newWorldRot's pitch & yaw
            fpc.playerCamera.rotation = rotation * Quaternion.Euler(pitch, 0f, 0f);
        }

        // Re-enable controller next frame (give physics time to resolve)
        yield return null;
        cc.enabled = true;

        // Give the player exit velocity via FirstPersonControls.AddPortalExitVelocity if available
        if (fpc != null)
        {
            fpc.AddPortalExitVelocity(exitVelocity);
        }
        else
        {
            // As fallback, directly move a small amount in the direction of exitVelocity
            cc.Move(exitVelocity * Time.deltaTime);
        }

        yield break;
    }

    // Called by your Portals.OnTriggerEnter
    public void SetPortals(Portals inPortal, Portals outPortal)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;
        isInPortalZone = true;
        hadLastLocalPos = false;
    }

    // Called by Portals.OnTriggerExit
    public void ClearPortals(Portals inPortal, Portals outPortal)
    {
        // Ensure we're clearing the ones we set (defensive)
        if (this.inPortal == inPortal && this.outPortal == outPortal)
        {
            this.inPortal = null;
            this.outPortal = null;
            isInPortalZone = false;
            hadLastLocalPos = false;
        }
    }
}