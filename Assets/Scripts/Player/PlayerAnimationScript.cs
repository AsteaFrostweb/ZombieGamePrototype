using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerMovement playerMovement;
    private Animator playerModelAnimator;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerModelAnimator = transform.Find("Model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimatorVelocity();
    }

    private void UpdateAnimatorVelocity() 
    {
        float velocityRatio = Mathf.Min(1f, playerMovement.Speed / playerMovement.SprintSpeed);
        if (playerMovement.IsGrounded)
        {
            playerModelAnimator.SetFloat("Velocity", velocityRatio);
        }
    }
}
