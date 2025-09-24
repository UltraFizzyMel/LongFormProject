using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerInput.PortalsActions portals;
    private PlayerInput.MenuActions onMenu;
    private PlayerMotor motor;
    private PlayerLook look;
    private PortalGun portalGun;
    private Reset reset;

    public event Action<int> OnPortalShot;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        portals = playerInput.Portals;
        onMenu = playerInput.Menu;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        portalGun = GetComponent<PortalGun>(); 
        reset = GetComponent<Reset>();  

        
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Sprint.performed += ctx => motor.Sprint();

        portals.RedPortal.performed += ctx => portalGun.ShootPortal(0);
        portals.BluePortal.performed += ctx => portalGun.ShootPortal(1);
        onMenu.Reset.performed += ctx => reset.ReloadCurrentScene();
    }

    
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
        portals.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
        portals.Disable();  
    }
}
