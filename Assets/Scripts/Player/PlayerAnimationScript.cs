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
    [Range(1f, 100f)]
    public float smoothing;
  
    public float maxSpeed = 750f;

    [Header("Outputs")]
    public Vector3 currentLocalVelocity;
    



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
        Vector3 local_vel = playerMovement.playerLocalVelocity;

        float current_x, current_z;
        current_x = playerModelAnimator.GetFloat("VelocityX");
        current_z = playerModelAnimator.GetFloat("VelocityZ");

        float desired_x, desired_z;
        desired_x = local_vel.x / maxSpeed;
        desired_z = local_vel.z / maxSpeed;
        
        
        playerModelAnimator.SetFloat("VelocityX", Mathf.Lerp(current_x, desired_x, (1 / smoothing)));
        playerModelAnimator.SetFloat("VelocityZ", Mathf.Lerp(current_z, desired_z, (1 / smoothing)));      
    }

 
}
