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
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
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
    }

    private void FixedUpdate()
    {
        if(player.isWallRunning)
        {
            WallRunMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //getting inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //state 1: wallrunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
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
    }

    private void WallRunMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //forward force
        cc.Move(wallForward * wallRunForce);
    }

    private void StopWallRun()
    {
        player.isWallRunning = false;
    }
}
