using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonSliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private Transform _playerObj;
    private CharacterController _controller;
    private ThirdPersonMovement _player;

    [Header("Sliding")]
    //public float maxSlideTime;
    public float slideForce;
    //private float slideTimer;

    public float slideYScale;
    private float _startYScale;
    private bool _sliding;

    [Header("Input")]
    private bool _slidePressed;
    private float _horizontalInput;
    private float _verticalInput;
    public Vector3 storedInput;

    private void Awake()
    {
        _playerObj = transform;
        _controller = GetComponent<CharacterController>();
        _player = GetComponent<ThirdPersonMovement>();

        _startYScale = _playerObj.localScale.y;
    }

    private void Update()
    {
        //a + d input (left + right)
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        //w + s input (forward + backwards)
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (_slidePressed && _controller.isGrounded/*&& (horizontalInput != 0 || verticalInput != 0)*/) StartSlide();
        else if ((!_slidePressed && _sliding) || (_player.isJumping && _sliding))
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (_sliding) SlidingMovement();
    }

    private void StartSlide()
    {
        _sliding = true;
        _player.isSliding = true;

        _playerObj.localScale = new Vector3(_playerObj.localScale.x, slideYScale, _playerObj.localScale.z);
        _controller.Move(Vector3.down * 100);
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection;
        if (_verticalInput == 0 && _horizontalInput == 0)
        {
            inputDirection = orientation.forward;
        }
        else
        {
            inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        }

        _controller.Move(inputDirection.normalized * slideForce);
        storedInput = _controller.velocity;
    }

    private void StopSlide()
    {
        _sliding = false;
        _player.isSliding = false;

        _playerObj.localScale = new Vector3(_playerObj.localScale.x, _startYScale, _playerObj.localScale.z);
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        _slidePressed = context.performed;
    }
}
