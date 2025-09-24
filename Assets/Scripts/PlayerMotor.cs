using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    public Vector3 playerVelocity;
    public float speed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 2f;
    private bool isGrounded;
    private bool sprinting;
    public bool isInputEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
    }
    //receive input from inputmanager script and apply to character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;  
        moveDirection.z = input.y;  
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (isGrounded && playerVelocity.y < 0 ) 
        {
            playerVelocity.y = -2f;
            isInputEnabled = true; 
        }

    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
        {
            speed = 8;
        }

        else
        {
            speed = 5;
        }
    }
}
