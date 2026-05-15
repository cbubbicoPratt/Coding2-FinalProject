using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonSliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private Transform _playerObj;
    private CharacterController _controller;
    private ThirdPersonMovement _player;
    public Transform camTransform;

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
        if (ButtonManager.startScreen && GameObject.FindFirstObjectByType<ButtonManager>() != null) return;
        //a + d input (left + right)
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        //w + s input (forward + backwards)
        _verticalInput = Input.GetAxisRaw("Vertical");

        //slide when grounded, stop if we don't press button or are jumping
        if (_slidePressed && _controller.isGrounded/*&& (horizontalInput != 0 || verticalInput != 0)*/) StartSlide();
        else if ((!_slidePressed && _sliding) || (_player.isJumping && _sliding))
        {
            StopSlide();
        }
        if (_sliding) SlidingMovement();
    }

    private void StartSlide()
    {
        _sliding = true;
        //enable sliding motion on player movement script
        _player.isSliding = true;

        //diminish scale and apply downward force
        _playerObj.localScale = new Vector3(_playerObj.localScale.x, slideYScale, _playerObj.localScale.z);
        _controller.Move(Vector3.down * 20);
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection;

        //without input we just want the player to move forward towards where the camera is pointing
        if (_verticalInput == 0 && _horizontalInput == 0)
        {
            inputDirection = camTransform.forward;
            _controller.transform.rotation = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);
        }
        //otherwise follow player input
        else
        {
            inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        }

        //now move and store input for jumping
        _controller.Move(inputDirection.normalized * slideForce);
        storedInput = _controller.velocity;
    }

    private void StopSlide()
    {
        _sliding = false;
        _player.isSliding = false;

        //resetscale
        _playerObj.localScale = new Vector3(_playerObj.localScale.x, _startYScale, _playerObj.localScale.z);
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        _slidePressed = context.performed;
    }
}
