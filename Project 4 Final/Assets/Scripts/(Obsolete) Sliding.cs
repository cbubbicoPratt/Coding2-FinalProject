using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private CharacterController cc;
    private CCPlayer ccPlayer;

    [Header("Sliding")]
    //public float maxSlideTime;
    public float slideForce;
    //private float slideTimer;

    public float slideYScale;
    private float startYScale;
    private bool sliding;

    [Header("Input")]
    private bool slidePressed;
    private float horizontalInput;
    private float verticalInput;
    public Vector3 storedInput;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        ccPlayer = GetComponent<CCPlayer>();
        
        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        //a + d input (left + right)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //w + s input (forward + backwards)
        verticalInput = Input.GetAxisRaw("Vertical");

        if (slidePressed && cc.isGrounded/*&& (horizontalInput != 0 || verticalInput != 0)*/) StartSlide();
        else if ((!slidePressed && sliding) || (ccPlayer.isJumping && sliding))
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (sliding) SlidingMovement();
    }

    private void StartSlide()
    {
        sliding = true;
        ccPlayer.isSliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        cc.Move(Vector3.down);
    }

    private void SlidingMovement()
    { 
        Vector3 inputDirection;
        if (verticalInput == 0 && horizontalInput == 0)
        {
            inputDirection = orientation.forward;
        }
        else
        {
            inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }

        cc.Move(inputDirection.normalized * slideForce);
        storedInput = cc.velocity;
    }

    private void StopSlide()
    {
        sliding = false;
        ccPlayer.isSliding = false;

        playerObj.localScale = new Vector3 (playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        slidePressed = context.performed;
    }
}
