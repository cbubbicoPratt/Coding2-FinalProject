using System.Collections;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float verticalInput;
    private float horizontalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    //public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;

    [Header("References")]
    public Transform cameraHolder;
    public Transform orientation;
    private CCPlayer player;
    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        player = GetComponent<CCPlayer>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
        cameraHolder.localEulerAngles = new Vector3(0, 0, cameraHolder.localEulerAngles.z);
        //Debug.Log(cameraHolder.localEulerAngles);
    }
    

    /*
    private void FixedUpdate()
    {
        if(player.isWallRunning)
        {
            WallRunMovement();
        }
    }
    */

    //raycast on either side to check walls
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    /*
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }
    */

    //checks if we are wallrunning
    private void StateMachine()
    {
        //getting inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //state 1: wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && !cc.isGrounded)
        {
            //start wallrun!
            if (!player.isWallRunning)
            {
                StartWallRun();
            }
        }

        //state 2: none
        else 
        {
            if (player.isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        //Debug.Log("Started");
        player.isWallRunning = true;
        //turn camera holder right
        if(wallRight)
        {
            StartCoroutine(CameraCoroutine(new Vector3(0, 0, 20), 0.2f));
        } 
        //turn camera holder left
        else if (wallLeft)
        {

            StartCoroutine(CameraCoroutine(new Vector3(0, 0, -20), 0.2f));
        }
    }

    /*
    private void WallRunMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        cc.Move(wallForward * wallRunForce);
    }
    */

    private void StopWallRun()
    {
        //Debug.Log("Stopped");
        player.isWallRunning = false;

        //when we stop wallrunning, we want the camera to lerp back to its original position
        //if its to the left, it has to be 360 so it doesnt spin all the way around
        //otherwise its zero
        if (cameraHolder.localEulerAngles.z > 180) StartCoroutine(CameraCoroutine(new Vector3(0, 0, 360), 0.2f));
        else StartCoroutine(CameraCoroutine(new Vector3(0, 0, 0), 0.2f));
    }

    //coroutine to move camera from where it is to a new position
    IEnumerator CameraCoroutine(Vector3 targetRotation, float duration)
    {
        Vector3 initialRotation = cameraHolder.localEulerAngles;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            cameraHolder.localEulerAngles = Vector3.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        //hard code camera to final position
        cameraHolder.localEulerAngles = targetRotation;
    }
}
