//using UnityEditor.ShaderGraph;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    public Rigidbody rb;
    private PlayerMovement playerMovement;
    private FirstPersonControls firstPersonControls;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("CameraEffects")]
    //public PlayerCam cam;
    public float dashFOV;


    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCooldown;
    private float dashCdTimer;

    [Header("Input")]
    public KeyCode heldDashKey = KeyCode.LeftShift;
    public KeyCode tapDashKey = KeyCode.LeftControl;
    //public KeyCode dashKey = KeyCode.E;

    private void Start()
    {
        rb.GetComponent<Rigidbody>();
        firstPersonControls = GetComponent<FirstPersonControls>();
    }

    private void Update()
    {
        /*if (Input.GetKey(heldDashKey))
        {
            if (Input.GetKeyDown(tapDashKey))
                Dash();
        }*/

        /*if (Input.GetKeyDown(KeyCode.E))
        {
            Dash();
        }*/

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
    }

    public void Dash()
    {
        if (dashCdTimer > 0)
            return;
        else
            dashCdTimer = dashCooldown;

        //firstPersonControls.dashing = true;
        //firstPersonControls.maxYSpeed = maxDashYSpeed;
        //Debug.Log("Dashing");

        //cam.DoFOV(dashFOV);

        Transform forwardT;

        if (useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
        {
            rb.velocity = Vector3.zero;
        }
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        //firstPersonControls.dashing = false;
        //firstPersonControls.maxYSpeed = 0;

        //cam.DoFOV(50f);

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        /*float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;*/

        Vector2 input = firstPersonControls.moveInput;

        Vector3 direction = forwardT.forward * input.y + forwardT.right * input.x;

        if (input == Vector2.zero)
            direction = forwardT.forward;

        return direction.normalized;
    }
}
