using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonWallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("Input")]
    private float _verticalInput;
    private float _horizontalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    //public float minJumpHeight;
    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    public bool wallLeft;
    public bool wallRight;
    private bool _wait = true;
    private bool _buffer = true;

    [Header("References")]
    //public Transform cameraHolder;
    public Transform orientation;
    private ThirdPersonMovement _player;
    private CharacterController _controller;
    public Image wallBar;


    private void OnEnable()
    {
        //reset wall timer when we reset position
        ThirdPersonMovement.onReset += WallTimerReset;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _player = GetComponent<ThirdPersonMovement>();
        wallRunTimer = maxWallRunTime;
    }
    private void Update()
    {
        if (ButtonManager.startScreen && GameObject.FindFirstObjectByType<ButtonManager>() != null) return;
        CheckForWall();
        StateMachine();
        WallRunStamina();

        //keep wall run time within range
        if (wallRunTimer < 0) wallRunTimer = 0;
        if (wallRunTimer > maxWallRunTime) wallRunTimer = maxWallRunTime;

        //fill out bar accordingly
        wallBar.fillAmount = wallRunTimer / maxWallRunTime;
    }

    private void CheckForWall()
    {
        //casts to check for walls on sides of player
        wallRight = Physics.Raycast(transform.position, orientation.right, out _rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out _leftWallHit, wallCheckDistance, whatIsWall);
    }

    private void StateMachine()
    {
        //getting inputs
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        //state 1: wallrunning
        if ((wallLeft || wallRight) && _verticalInput > 0 && !_controller.isGrounded && wallRunTimer > 0)
        {
            //start wallrun!
            if (!_player.isWallRunning)
            {
                StartWallRun();
            }
        }

        //state 2: none
        else
        {
            if (_player.isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        _player.isWallRunning = true;
    }

    private void StopWallRun()
    {
        _player.isWallRunning = false;
    }

    private void WallRunStamina()
    {
        //buffer starts whenever player isn't grounded
        if (!_controller.isGrounded) _buffer = true;
        //decrease stamina
        if (_player.isWallRunning)
        {
            wallRunTimer -= 2 * Time.deltaTime;
            //wait to refill if we start wallrunning
            _wait = true;
        }
        //refill stamina after buffer
        else if (wallRunTimer < maxWallRunTime)
        {
            if (_controller.isGrounded && _buffer) StartCoroutine(SecondBuffer());
            if (!_wait)
            {
                wallRunTimer += 3 * Time.deltaTime;
            }
        }
    }

    IEnumerator SecondBuffer()
    {
        //same logic as dash stamina
        //first wait, then turn off logic so stamina refills
        _wait = true;
        yield return new WaitForSeconds(1);
        //if we are wallrunning during buffer, stop logic
        if (_player.isWallRunning) yield break;
        _buffer = false;
        _wait = false;
        yield break;
    }

    private void WallTimerReset(bool doReset)
    {
        wallRunTimer = maxWallRunTime;
    }
}
