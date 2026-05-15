using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    private CharacterController _controller;
    public Transform cam;
    private ThirdPersonWallRunning _tpwr;
    private ThirdPersonSliding _tps;

    [Header("Movement")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 5f;
    private float _verticalVelocity;
    private float _gravity = -30f;

    [Header("Detection")]
    private float _targetRotation;
    private float _turnSmoothVelocity;
    public bool isJumping = false;
    public bool isWallRunning;
    public bool isSliding;
    private Vector2 _moveInput;
    private Vector3 _startPos;

    //event to call when position is reset
    public static event Action<bool> onReset;
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = transform.position;
        _tpwr = GetComponent<ThirdPersonWallRunning>();
        _tps = GetComponent<ThirdPersonSliding>();

        if (GameObject.FindFirstObjectByType<ButtonManager>() == null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (ButtonManager.startScreen && GameObject.FindFirstObjectByType<ButtonManager>() != null) return;
        HandleMove();

        //reset position if player falls off map
        if(transform.position.y < -20) ResetPosition();
    }

    public void HandleMove()
    {
        //constant down force
        if(_controller.isGrounded && _verticalVelocity <= 0)
        {
            _verticalVelocity = -2;
        }

        //jump handler
        if (isJumping && _controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        } else if (isJumping && isWallRunning)
        {
            //move player off the wall depending on what side theyre on
            //then apply upward force
            if (_tpwr.wallLeft)
            {
                _controller.Move(transform.right * (175 * Time.deltaTime));
            }
            else if (_tpwr.wallRight)
            {
                _controller.Move(-transform.right * (175 * Time.deltaTime));
            }
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        } else if (isJumping && isSliding)
        {
            //jump out of slide (wasn't working!!!)
            isSliding = false;
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * _gravity);
            _controller.Move(_tps.storedInput);
        } else isJumping = false;

        //convert movement input to vector3
        Vector3 inputDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        Vector3 moveDir;
        
        //Vector3 move = (transform.right * moveInput.x * speed + transform.forward * moveInput.y * speed).normalized;

        //if we are moving, first target the direction we are moving and rotate to that point using Atan2 to find the shortest path
        //use smoothdampangle to smoothly rotate to the target rotation over time (while input is held)
        //finally, move in that direction
        if (_moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;
        } else moveDir = Vector3.zero;

        //calculate vertical force
        if (!isWallRunning) _verticalVelocity += _gravity * Time.deltaTime;
        Vector3 velocity = Vector3.up * _verticalVelocity;
        
        //finally, move!
        _controller.Move(moveDir * (speed * Time.deltaTime) + velocity * Time.deltaTime);

        //disable gravity if wallrunning
        if(isWallRunning && !isJumping) _verticalVelocity = 0f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) isJumping = true;
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if(context.performed) ResetPosition();
    }

    public void ResetPosition()
    {
        //disable controller to move player and then broadcast event to reset stamina/wall timer
        _controller.enabled = false;
        transform.position = _startPos;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _controller.enabled = true;
        onReset?.Invoke(true);
    }
}
