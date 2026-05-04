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
    public Transform camera;
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
        //Debug.Log(camera.localEulerAngles);
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

        //state 3: none
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
        player.isWallRunning = true;
        /*if(wallRight)
        {
            StartCoroutine(CameraCoroutine(new Vector3(camera.localEulerAngles.x, camera.localEulerAngles.y, 20), 0.2f));
        } 
        else if (wallLeft)
        {
            StartCoroutine(CameraCoroutine(new Vector3(camera.localEulerAngles.x, camera.localEulerAngles.y, -20), 0.2f));
        }*/
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
        player.isWallRunning = false;
        //StartCoroutine(CameraCoroutine(new Vector3(camera.localEulerAngles.x, camera.localEulerAngles.y, 0), 0.2f));
    }

    /*IEnumerator CameraCoroutine(Vector3 targetRotation, float duration)
    {
        Vector3 initialRotation = camera.localEulerAngles;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            camera.localEulerAngles = Vector3.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        camera.localEulerAngles = targetRotation;
    }*/
}
