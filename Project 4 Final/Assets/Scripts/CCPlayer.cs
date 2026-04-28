using JetBrains.Annotations;
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

    [Header("Components")]
    public Transform cameraTransform;
    public Transform dashTarget;
    public Image staminaBar;

    private CharacterController cc;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalVelocity; //current upward/downward speed
    private float gravity = -20; //constant downward acceleration
    private float pitch; //up and down
    public float stamina = 1;
    public float maxStamina = 1;

    public bool isDashing;
    public bool isJumping;
    private bool wait;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        //optional cursor locking
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
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
        Vector3 dashMove = dashTarget.transform.forward * 0.5f;

        if (isDashing && stamina > 0)
        {
            cc.Move(dashMove);
            stamina -= 3 * Time.deltaTime;
        }
        Debug.Log(stamina);
        //if jumping is true and we are grounded
        if (isJumping && grounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            isJumping = false;
        }

        if(grounded && stamina < maxStamina && !isDashing)
        {
            if (stamina == 0) StartCoroutine(SecondBuffer());
            if(!wait) stamina += 2 * Time.deltaTime;
        }
        if (!isDashing || stamina <= 0)
        {
            //apply gravity to every frame
            verticalVelocity += gravity * Time.deltaTime;
            //convert vertical velocity into movement vector
            Vector3 velocity = Vector3.up * verticalVelocity;
            //NOW we are finally moving player
            cc.Move((move + velocity) * Time.deltaTime);
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
        //can only jump if the menu isn't open (prevents jumping after space is pressed in the menu and then exiting)
        if (context.performed && Cursor.lockState == CursorLockMode.Locked) isJumping = true;
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
        wait = false;
        yield break;
    }
}

