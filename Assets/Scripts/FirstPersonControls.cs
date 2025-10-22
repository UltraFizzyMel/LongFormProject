
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static NewControls;

public class FirstPersonControls : MonoBehaviour
{

    [Header("MOVEMENT SETTINGS")]
    [Space(5)]
    // Public variables to set movement and look speed, and the player camera
    public float moveSpeed; // Speed at which the player moves
    public float currentMoveSpeed;
    public float lookSpeed; // Sensitivity of the camera movement
    public float gravity = -9.81f; // Gravity value
    public float jumpHeight = 1.5f; // Height of the jump
    public Transform playerCamera; // Reference to the player's camera
    public Camera cam;

    // Private variables to store input values and the character controller
    public Vector2 moveInput; // Stores the movement input from the player
    private Vector2 lookInput; // Stores the look input from the player
    private float verticalLookRotation = 0f; // Keeps track of vertical camera rotation for clamping
    public Vector3 velocity; // Velocity of the player
    private CharacterController characterController; // Reference to the CharacterController component
    //public Rigidbody rb;
    public bool canPlayerMove = true;

  

    private NewControls playerInput;


    


    [Header("CROUCH HEIGHT SETTINGS")]
    [Space(5)]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 0.5f;
    private bool isCrouching = false;

    [Header("SPRINT")]
    public float sprintSpeed = 10f;
    public float currentSprintSpeed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private float staminaDrainRate = 2f;
    [SerializeField] private float regenCooldown = 1.5f;
    public bool isSprinting;
    public bool canSprint = false;
    private float currentStamina;
    private float lastSprintTime;

   

    [Header("DASH")]
    public float dashForce = 20f;
    public float dashUpwardForce = 0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashFOV;

    public bool isDashing = false;
    public bool readyToDash = true;

    public Dashing dashingScript;

    [Header("PORTAL")]
    public PortalGun portalGun;
    public event Action<int> OnPortalShot;
    private PlayerInput.OnFootActions onFoot;
    private NewControls.PortalsActions portals;

    [Header("TRUE VELOCITY CALCULATION")]
    private Vector3 previousPosition;
    public Vector3 trueVelocity;

    [Header("PORTAL VELOCITY")]
    public Vector3 portalVelocity; // Separate portal exit velocity
    public float portalVelocityDecay = 3f; // How quickly portal velocity fades


    public enum WeaponType { PortalGun, FreezeGun }
    public WeaponType currentWeapon = WeaponType.PortalGun;
    public FreezeGun freezeGun; // Reference to your FreezeGun script

    private NewControls.FreezeGunActions freezeGunActions;




    private void Awake()
    {
        playerInput = new NewControls();
        portals = playerInput.Portals;
        freezeGunActions = playerInput.FreezeGun;
        // Get and store the CharacterController component attached to this GameObject
        characterController = GetComponent<CharacterController>();
        
        //portals.RedPortal.performed += ctx => { if (currentWeapon == WeaponType.PortalGun)  portalGun.ShootPortal(0); };
        // portals.BluePortal.performed += ctx => { if (currentWeapon == WeaponType.PortalGun) portalGun.ShootPortal(1); };




    }

    public void Start()
    {
        currentMoveSpeed = moveSpeed;
        currentSprintSpeed = sprintSpeed;
        previousPosition = transform.position;
    }

    private void OnEnable()
    {
        // Create a new instance of the input actions
        playerInput = new NewControls();
        portals = playerInput.Portals;
        freezeGunActions = playerInput.FreezeGun;


        // Enable the input actions
        playerInput.Player.Enable();

        // Subscribe to the movement input events
        playerInput.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // Update moveInput when movement input is performed
        playerInput.Player.Movement.canceled += ctx => moveInput = Vector2.zero; // Reset moveInput when movement input is canceled

        // Subscribe to the look input events
        playerInput.Player.LookAround.performed += ctx => lookInput = ctx.ReadValue<Vector2>(); // Update lookInput when look input is performed
        playerInput.Player.LookAround.canceled += ctx => lookInput = Vector2.zero; // Reset lookInput when look input is canceled

        // Subscribe to the jump input event
        playerInput.Player.Jump.performed += ctx => Jump(); // Call the Jump method when jump input is performed

        // Subscribe to the crouch input event
        //playerInput.Player.Crouch.performed += ctx => ToggleCrouch(); // Call the Crouch method when crouch input is performed

        playerInput.Player.Sprint.performed += ctx => StartSprint();

        playerInput.Player.Sprint.canceled += ctx => StopSprint();

        // Weapon switching
        playerInput.Player.SwitchWeapons.performed += ctx => SwitchWeapon();


        //FreeezeGun
        freezeGunActions.Enable();
        freezeGunActions.Shoot.performed += ctx => { if (currentWeapon == WeaponType.FreezeGun) freezeGun.shootBullet(); };


        // Subscribe to the jump input event
        playerInput.Player.Dash.performed += ctx => Dash(); // Call the Jump method when jump input is performed

       

        playerInput.Menu.Reset.performed += ctx => ReloadCurrentScene();

        //portals 
        portals.Enable();
        portals.RedPortal.performed += ctx => { if (currentWeapon == WeaponType.PortalGun) portalGun.ShootPortal(0); };
        portals.BluePortal.performed += ctx => { if (currentWeapon == WeaponType.PortalGun) portalGun.ShootPortal(1); };



    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        portals.Disable();
        freezeGunActions.Disable();
    }

    private void Update()
    {
        if (canPlayerMove)
        {
            // Calculate true velocity BEFORE any movement happens
            

            // Call Move and LookAround methods every frame to handle player movement and camera rotation
            Move();
            LookAround();
            ApplyGravity();
            UpdateStamina();
            
            ApplyPortalVelocity();

            //CalculateTrueVelocity();
        }

        
    }

    private void CalculateTrueVelocity()
    {
        // Calculate velocity based on position change
        trueVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

    }

    // Public method to get the true velocity
    public Vector3 GetTrueVelocity()
    {
        return trueVelocity;

    }

    public void ApplyPortalVelocity()
    {
        if (portalVelocity.magnitude > 0.1f)
        {
            // CRITICAL FIX: Stop portal velocity when player is grounded
            if (characterController.isGrounded)
            {
                Debug.Log("player on ground");
                // Apply strong friction when grounded
                portalVelocity = Vector3.MoveTowards(portalVelocity, Vector3.zero, portalVelocityDecay * 3f * Time.deltaTime);

                // Extra: if moving very slowly on ground, stop completely
                if (portalVelocity.magnitude < 2f)
                {
                    portalVelocity = Vector3.zero;
                    return;
                }
            }

            // Apply portal velocity
            characterController.Move(portalVelocity * Time.deltaTime);

            // Normal decay in air
            portalVelocity = Vector3.MoveTowards(portalVelocity, Vector3.zero, portalVelocityDecay * Time.deltaTime);
        }
        else if (portalVelocity.magnitude > 0f)
        {
            portalVelocity = Vector3.zero;
        }


    }

    public void OverwritePreviousPosition(Vector3 pos)
    {
        previousPosition = pos;
        trueVelocity = Vector3.zero;
    }

    // Force-set the character's current velocity vector (used after teleport)
    public void SetVelocity(Vector3 v)
    {
        velocity = v;
    }

    public void AddPortalExitVelocity(Vector3 exitVelocity)
    {
        portalVelocity = exitVelocity;
        //velocity = exitVelocity;
    }

    public void Move()
    {
        // Create a movement vector based on the input
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Transform direction from local to world space
        move = transform.TransformDirection(move);

        float currentSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        // Move the character controller based on the movement vector and speed
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    public void LookAround()
    {
        // Get horizontal and vertical look inputs and adjust based on sensitivity
        float LookX = lookInput.x * lookSpeed;
        float LookY = lookInput.y * lookSpeed;

        // Horizontal rotation: Rotate the player object around the y-axis
        transform.Rotate(0, LookX, 0);

        // Vertical rotation: Adjust the vertical look rotation and clamp it to prevent flipping
        verticalLookRotation -= LookY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        // Apply the clamped vertical rotation to the player camera
        playerCamera.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
    }

    public void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f; // Small value to keep the player grounded
        }

        velocity.y += gravity * Time.deltaTime; // Apply gravity to the velocity
        characterController.Move(velocity * Time.deltaTime); // Apply the velocity to the character
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            // Calculate the jump velocity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        }
    }

    public void ToggleCrouch()
    {
        if (isCrouching)
        {
            //Stand up
            //characterController.height = standingHeight;
            //isCrouching = false;
        }
        else
        {
            //Crouch down
            //characterController.height = crouchHeight;
            //isCrouching = true;
        }
    }

    private void Dash()
    {
        if (!readyToDash) return;

        readyToDash = false;
        isDashing = true;

        Vector3 dashDirection = GetDashDirection();

        // Move instantly in the dash direction
        StartCoroutine(PerformDash(dashDirection));
    }

    private Vector3 GetDashDirection()
    {
        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private IEnumerator PerformDash(Vector3 dashDirection)
    {
        float startTime = Time.time;
        //cam.DOFieldOfView(dashFOV, 0.25f);

        cam.DOFieldOfView(dashFOV, dashDuration / 2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            cam.DOFieldOfView(50f, dashDuration / 2f).SetEase(Ease.InQuad);
        });

        while (Time.time < startTime + dashDuration)
        {
            characterController.Move(dashDirection * dashForce * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
        Invoke(nameof(ResetDashCooldown), dashCooldown);
    }

    private void ResetDashCooldown()
    {
        //cam.DOFieldOfView(50f, 0.25f);
        readyToDash = true;
    }

    private void StartSprint()
    {
        if (canSprint)
        {
            if (currentStamina > 0)
            {
                isSprinting = true;
            }
        }
    }

    private void StopSprint()
    {
        isSprinting = false;
        lastSprintTime = Time.time; // Start cooldown
    }

    private void UpdateStamina()
    {
        if (isSprinting && currentStamina > 0)
        {
            // Drain stamina while sprinting
            currentStamina -= staminaDrainRate * Time.deltaTime;
            lastSprintTime = Time.time;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                StopSprint(); // Auto-stop when exhausted
            }
        }
        else if (Time.time > lastSprintTime + regenCooldown && currentStamina < maxStamina)
        {
            // Regen stamina after cooldown
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    private void SwitchWeapon()
    {
        if (currentWeapon == WeaponType.PortalGun)
        {
            SwitchToWeapon(WeaponType.FreezeGun);
        }
        else
        {
            SwitchToWeapon(WeaponType.PortalGun);
        }
    }

    private void SwitchToWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;

        // Update UI or visual indicators here if needed
        Debug.Log($"Switched to: {weapon}");

        // You could also trigger weapon model switching here
        // UpdateWeaponVisuals();
    }

    public void ReloadCurrentScene()
    {
        // Get the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }

}

