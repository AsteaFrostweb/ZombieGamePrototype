using Assets.Scripts.Utility;
using Game.Utility;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;


public class PlayerMovement : MonoBehaviour
{
    //Inspector variables
    [Header("Movement")]
    public float MoveSpeed = 2.5f;
    public float SprintSpeed = 7.5f;
    public float speedDampeningRate = 10f;
    public float jumpHeight = 2f;
    public float jumpBoxHeight = 0.2f;

    public float vert_presseed = 0f;

    [Header("View")]
    public Vector2 mouseSpeed = Vector2.one;
    public float lookAnglecap = 75f;

    [Header("Outputs")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool canJump;
    [SerializeField]
    private bool isSprinting;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Vector3 nonGroundedMoveCache;
    [SerializeField]
    private Vector3[] pastVelocities;
    public float Speed { get { return speed; } }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsSprinting { get { return isSprinting; } }


    //Private variables
    private Transform playerHead;
    private CharacterController controller;    
    public Vector3 playerVelocity { get; private set; }
    public CircularBuffer<Vector3> pastVelocityBuffer { get; private set; }
    public int pastVelocityBufferSize { get; private set; } = 8;

    //Change trackers
    private ChangeTracker<bool> sprintTracker;
    private ChangeTracker<bool> groundedTracker;

    //Events
    public event Action OnSprintStart;
    public event Action OnSprintEnd;
    public event Action OnJump;
    public event Action OnLand;

    //private input varibles
    float mouseX;
    float mouseY;

    private void Start()
    {
        //Find objects
        playerHead = transform.Find("HeadContainer");
        controller = GetComponent<CharacterController>();

        //Initialize change trackers
        sprintTracker = new ChangeTracker<bool>(() => isSprinting);
        groundedTracker = new ChangeTracker<bool>(() => isGrounded);

        //Initalize previous move array
        pastVelocityBuffer = new CircularBuffer<Vector3>(pastVelocityBufferSize);  

        //Assigning Events
        OnJump += () => Jump();
        OnLand += () => Land();
    }



    void Update()
    {
        pastVelocities = pastVelocityBuffer.buffer;


        CheckGrounded(8, controller.radius);
           
        GetEvents();
       
        GetMovementInput();      
        GetRotationInput();
          

        ApplyGravity();

      

   

    }

    private void FixedUpdate()
    {
        HandleGrounded();

        HandleMovement();

        HandleRotation();
        
    }

    private void GetEvents()
    {
        isSprinting = Input.GetButton("Sprint");

        if (Input.GetButtonDown("Jump") && canJump)
        {
            OnJump.Invoke();
        }

        if (sprintTracker.Update()) //returns true if the "isSprinting" value is different than last frame
        {
            if (isSprinting && OnSprintStart != null) OnSprintStart.Invoke();
            else if (OnSprintEnd != null) OnSprintEnd.Invoke();
        }

    }


    private void GetMovementInput() 
    {
        float moveSpeed = isSprinting ? SprintSpeed : MoveSpeed;       

        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * moveSpeed * Time.deltaTime;       

        Vector3 move = new Vector3(inputs.x, 0, inputs.y);
        move = transform.TransformDirection(move);
        move = new Vector3(move.x, 0f, move.z);
        if (isGrounded)
        {
            if (groundedTracker.Update())
            {
                Debug.Log("Player Landed on Ground");
            }
            playerVelocity += move;
            pastVelocityBuffer.Append(move);
        }
        else
        {
            if(groundedTracker.Update())
            {
                Debug.Log("Player Left Ground");
                nonGroundedMoveCache = GameMath.AverageVector(pastVelocityBuffer.buffer);
                playerVelocity += new Vector3(nonGroundedMoveCache.x, 0f, nonGroundedMoveCache.z);
            }
            
            playerVelocity += new Vector3(nonGroundedMoveCache.x, 0f, nonGroundedMoveCache.z);
        }
    }
    private void HandleMovement() 
    {
        controller.Move(playerVelocity * Time.fixedDeltaTime);

        velocity = playerVelocity;       
        speed = (new Vector3(playerVelocity.x, 0f, playerVelocity.z) / Time.fixedDeltaTime).magnitude;

        playerVelocity = new Vector3(0f, playerVelocity.y, 0f);      
    }


    private void GetRotationInput() 
    {
        mouseX += Input.GetAxis("Mouse X");
        mouseY += Input.GetAxis("Mouse Y");
    }
    private void HandleRotation() 
    {         
        transform.Rotate(Vector3.up * mouseX * mouseSpeed.x * Time.fixedDeltaTime); //rotate player y by mouseX
        mouseX = 0f;
        
        Vector3 currentRotation = playerHead.localEulerAngles;
        currentRotation.x -= mouseY * mouseSpeed.y * Time.fixedDeltaTime;
        currentRotation.x = GameMath.ClampEulerAngle(currentRotation.x, -lookAnglecap, lookAnglecap);
        playerHead.localEulerAngles = currentRotation;
        mouseY = 0f;
    }

    


    private void Jump() 
    {
        if (playerVelocity.y <= 0f)
        {
            float jumpImpulse = Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
            playerVelocity += new Vector3(playerVelocity.x, jumpImpulse, playerVelocity.z);
        }
    }
    private void Land()
    {
        //play sound fx if was moving certan speed before collision
        //handle fall damage maybe
    }




    private void CheckGrounded(int numPoints, float radius)
    {
      
        Vector3 origin = transform.position;

       
        float angleStep = 360f / numPoints;

        int castHits = 0;
      
        for (int i = 0; i < numPoints; i++)
        {
            // Calculate the angle for this ray
            float angle = i * angleStep * Mathf.Deg2Rad;

            // Calculate the position of the raycast origin in the circle
            Vector3 circlePosition = origin + new Vector3(Mathf.Cos(angle) * radius, -controller.height/4, Mathf.Sin(angle) * radius);

            float height = jumpBoxHeight + (controller.height / 4);

            // Perform the raycast
            RaycastHit hit;
            if (Physics.Raycast(circlePosition, Vector3.down, out hit, height))
            {
                // If the raycast hits something, draw a green ray
                Debug.DrawRay(circlePosition, Vector3.down * hit.distance, Color.green);
                castHits++;
            }
            else
            {
                // If the raycast doesn't hit anything, draw a red ray
                Debug.DrawRay(circlePosition, Vector3.down * height, Color.red);
            }
        }

        if (castHits >= (int)(numPoints * 0.4f))
        {
            canJump = true;
        } else canJump = false;

        if (castHits >= 1)
        {
            isGrounded = true;
        } else isGrounded = false;            
    }
    private void HandleGrounded()
    {
        if (isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity = new Vector3(playerVelocity.x, -2.0f, playerVelocity.z);
        }
    }
    private void ApplyGravity()
    {
        playerVelocity += Physics.gravity * Time.deltaTime;
    }

    
}
