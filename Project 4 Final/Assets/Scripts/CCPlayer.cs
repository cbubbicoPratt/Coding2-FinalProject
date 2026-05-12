using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CCPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 9;
    public float jumpHeight = 5;
    public float lookSensitivity = 1;
    public float wallRunSpeed = 9;

    [Header("Components")]
    public Transform cameraTransform;
    public Transform cameraTargetTransform;
    public Transform orientation;
    public Image staminaBar;

    private WallRunning wallRunning;
    private Sliding sliding;
    private CharacterController cc;
    public Vector3 startPos;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalVelocity; //current upward/downward speed
    private float gravity = -20; //constant downward acceleration
    private float pitch; //up and down
    public float stamina = 1;
    public float maxStamina = 1;

    public bool isDashing;
    public bool isJumping;
    public bool isWallRunning;
    public bool isSliding;
    private bool buffer = false;
    private bool wait;
    
    private void Awake()
    {
        //getting our references
        cc = GetComponent<CharacterController>();
        wallRunning = GetComponent<WallRunning>();
        sliding = GetComponent<Sliding>();
        startPos = cc.transform.position;

        //optional cursor locking
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        if (cc.transform.position.y < -20) ResetPosition();
        if (stamina < 0) stamina = 0;
        if(stamina > maxStamina) stamina = maxStamina;
        staminaBar.fillAmount = stamina / maxStamina;
    }

    private void HandleLook()
    {
        //horizontal mouse movement rotates player
        float yaw = lookInput.x * lookSensitivity;
        //vertical mouse movement rotates camera
        float pitchDelta = lookInput.y * lookSensitivity;

        transform.Rotate(Vector3.up * yaw);

        //accumulate vertical rotation
        pitch -= pitchDelta;
        //clamp so we don't flip upside down
        pitch = Mathf.Clamp(pitch, -90, 90);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void HandleMovement()
    {
        //updating our bool to be true or false if the player is grounded
        bool grounded = cc.isGrounded;
        //Debug.Log("Is Grounded: " + grounded);

        //this keeps character controller snapped to ground
        if (grounded && verticalVelocity <= 0)
        {
            verticalVelocity = -2;
        }

        float currentSpeed = walkSpeed;

        Vector3 move = transform.right * moveInput.x * currentSpeed + transform.forward * moveInput.y * currentSpeed;
        Vector3 dashMove = cameraTargetTransform.transform.forward * 0.25f;

        //dash handler
        if (isDashing && stamina > 0)
        {
            //if we jump and dash at the same time, we move twice the amount and lose more stamina
            if (isJumping && grounded)
            {
                cc.Move(dashMove * 2);
                stamina -= 5 * Time.deltaTime;
                buffer = true;
            }
            else
            {
                //dash movement separate than regular
                //dashing consumes stamina
                //wait a second to refill stamina after dashing
                cc.Move(dashMove);
                stamina -= 2 * Time.deltaTime;
                buffer = true;
            }
        }
        //if jumping is true and we are grounded
        if (isJumping && grounded)
        {
            //jump height increased when dashing
            if (isDashing)
            {
                verticalVelocity = Mathf.Sqrt((jumpHeight * 2f) * -2f * gravity);
            }
            else
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        //if we wallrun and jump, we want the player to slightly move away from the wall depending on what side theyre on
        //otherwise the jump is the same
        else if (isJumping && isWallRunning)
        {
            if (wallRunning.wallLeft)
            {
                cc.Move(transform.right * (175 * Time.deltaTime));
                //Debug.Log(transform.position);
            }
            else if (wallRunning.wallRight)
            {
                cc.Move(-transform.right * (175 * Time.deltaTime));
                //Debug.Log(transform.position);
            }
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        if(isJumping && isSliding)
        {
            move += sliding.storedInput;
        }
        else
        {
            isJumping = false;
        }

        //coroutine for waiting a second
        //had to do some crazy logic to get the script to actually wait
        if(stamina < maxStamina && !isDashing)
        {
            //refill stamina
            if (buffer && !wait) StartCoroutine(SecondBuffer());
            if(!wait) stamina += 2 *Time.deltaTime;
        }
        //we dont want gravity when dashing or wall running
        if ((!isDashing || stamina <= 0) && !isWallRunning)
        {
            //apply gravity to every frame
            verticalVelocity += gravity * Time.deltaTime;
        }
            //convert vertical velocity into movement vector
            Vector3 velocity = Vector3.up * verticalVelocity;
            //NOW we are finally moving player
            cc.Move((move + velocity) * Time.deltaTime);
        //no vertical movement when wallrunning
        if(isWallRunning && !isJumping)
        {
            //Debug.Log(transform.position);
            verticalVelocity = 0;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //when the key is hit, isJumping = true
        if (context.performed) isJumping = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isDashing = context.performed;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("CC collided with: " + hit.gameObject.name);
    }

    IEnumerator SecondBuffer()
    {
        wait = true;
        yield return new WaitForSeconds(1);
        stamina += 0.01f;
        buffer = false;
        wait = false;
        yield break;
    }

    public void ResetPosition()
    {
        cc.enabled = false;
        transform.position = startPos;
        cc.enabled = true;
        stamina = maxStamina;
        wallRunning.wallRunTimer = wallRunning.maxWallRunTime;
    }
}

