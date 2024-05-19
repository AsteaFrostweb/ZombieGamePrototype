using Game.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class PlayerAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerMovement playerMovement;
    private Animator playerModelAnimator;
    private Game.Utility.CircularBuffer<Vector3> playerPositionBuffer;

    [Header("Inputs")]
    [Range(1f, 100f)]
    public float smoothing;  
    public float maxSpeed = 750f;
    [SerializeField]
    private Rig WeaponAimRig;
    [SerializeField]
    private float WeaponDrawTime = 2f;
    [SerializeField]

    [Header("Weapon Clipping variables. Handles stopping the weapon form going through walls")]
    private Vector2 WeaponClipping_DistanceMinMax;
    [SerializeField]
    private Vector3 WeaponClipping_RayOffset;
    [SerializeField]
    private Vector3 WeaponClipping_RayDirection;
    [SerializeField]
    private LayerMask WeaponClipping_LayerMask;

    [Header("Outputs")]    
    public Vector3 currentLocalVelocity;
    [SerializeField]
    private bool weaponClipping;
    
    public Animator PlayerAnimator { get { return playerModelAnimator;  } }
    public bool weaponOut { get { return WeaponAimRig.weight == 1; } }
    public bool weaponIn { get { return WeaponAimRig.weight == 0; } }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerModelAnimator = transform.Find("PlayerModel").GetComponent<Animator>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement) maxSpeed = playerMovement.SprintSpeed;
        else return;

        if (playerMovement.IsADS)
        {
            Check_GunClipping();
        }
        
        Handle_ADSPoseChange();
  
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

    private void Handle_ADSPoseChange() 
    {
        if (!weaponIn && !playerMovement.IsADS)
        {
            WeaponAimRig.weight -= (1 / WeaponDrawTime) * Time.deltaTime;
            if (WeaponAimRig.weight < 0) WeaponAimRig.weight = 0;
        }
        else if (!weaponOut && playerMovement.IsADS && !weaponClipping)
        {
           
            WeaponAimRig.weight += (1 / WeaponDrawTime) * Time.deltaTime;
            if (WeaponAimRig.weight > 1) WeaponAimRig.weight = 1;
            
        }

    }

    private void Check_GunClipping() 
    {
        weaponClipping = false;

        RaycastHit hit;
        float max_clippingDistance = WeaponClipping_DistanceMinMax.y;
        float min_clippingDistance = WeaponClipping_DistanceMinMax.x;

        Vector3 dir = transform.TransformDirection(WeaponClipping_RayDirection).normalized;
        Vector3 offset = transform.TransformDirection(WeaponClipping_RayOffset);

        if (Physics.Raycast(transform.position + offset, dir, out hit, max_clippingDistance, WeaponClipping_LayerMask)) 
        {
            Debug.DrawRay(transform.position + offset, dir * hit.distance, Color.red);
            weaponClipping = true;
            if (hit.distance <= min_clippingDistance) 
            {
                WeaponAimRig.weight = 0f;
                return;
            }

            float diff, ratio, max_delta;
            max_delta = max_clippingDistance - min_clippingDistance;
            diff = max_clippingDistance - hit.distance;
            ratio = diff / max_delta;
            ratio = 1 - ratio;
            if (ratio < WeaponAimRig.weight) WeaponAimRig.weight = ratio;
        }
        else 
        {
            Debug.DrawRay(transform.position + offset, dir * max_clippingDistance, Color.green);
        }
    }
}
