using Game.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerMovement playerMovement;
    private Animator playerModelAnimator;
    private Game.Utility.CircularBuffer<Vector3> playerPositionBuffer;

    [Header("inputs")] 
    [Range(0f, 1f)]
    public float smoothingX;
    [Range(0f, 1f)]
    public float smoothingZ;
    public float maxSpeed = 750f;

    [Header("Outputs")]
    public Vector3 currentVelocity;
    public Vector3 previousPosition;



    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerModelAnimator = transform.Find("Model").GetComponent<Animator>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement) maxSpeed = playerMovement.SprintSpeed;
        else return;

       

        UpdateAnimatorVelocity();

       
    }

    private void UpdateAnimatorVelocity() 
    {
        currentVelocity = (playerMovement.transform.position - previousPosition) / Time.deltaTime;

        playerModelAnimator.SetFloat("VelocityX", currentVelocity.x / maxSpeed);
        playerModelAnimator.SetFloat("VelocityZ", currentVelocity.z / maxSpeed);

        previousPosition = playerMovement.transform.position;
    }

    private void UpdateAnimatorVelocityD()
    {
        

      
       
    }
}
