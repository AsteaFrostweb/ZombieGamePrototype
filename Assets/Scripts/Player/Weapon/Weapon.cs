using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Game.Weapon;


namespace Game.Weapon 
{
    [Serializable]
    public struct WeaponStats 
    {
        public float rpm;
        public float Damage;
        public float Penatration;
        public float Range;
        public float Speed; //715 m/s muzzle velocity of AK-47
        public float DamageFalloff;
        public float HeightFalloff;
   
    }
}

public class Weapon: MonoBehaviour
{

    private struct Player 
    {
        public GameObject obj;
        public PlayerMovement movement;
        public PlayerAnimationScript animation;

        private bool isNull;
       
     
        public bool IsNull() 
        {
            if (!isNull) return false;

            return (obj == null) || (movement == null) || (animation == null);
        }

        public Player(GameObject _obj, PlayerMovement _movement, PlayerAnimationScript _animation) 
        {
            obj = _obj;
            movement = _movement;
            animation = _animation;

            isNull = true;
        }
    }

    [SerializeField]
    private WeaponStats weapon_stats;
    [SerializeField]
    private ParticleSystem Muzzle_ParticleSystem;


    private Player player;
    private Animator playerAnimator;

    private float timeSinceFire;

    public float RPS { get { return weapon_stats.rpm / 60; } }
    public bool CanFire { get { return timeSinceFire >= (1 / RPS);  } }
    

    public int bullets_fired = 0;
    private void Start()
    {
        player = new Player(GameObject.Find("Player"), null, null);
      
    }
    // Update is called once per frame
    void Update()
    {
        timeSinceFire += Time.deltaTime;

        if (player.IsNull()) 
        {
            if(player.obj == null) player = new Player(GameObject.Find("Player"), null, null);
            if(player.obj == null) return;

            player.movement = player.obj.GetComponent<PlayerMovement>();
            player.animation = player.obj.GetComponent<PlayerAnimationScript>();
            if (player.IsNull()) return;
        }

        if (Input.GetButton("Fire1") && player.animation.weaponOut && CanFire) 
        {
            timeSinceFire = 0;
            bullets_fired++;

            Muzzle_ParticleSystem.Emit(1);
            PlayFire1Animation();
          
        };   
    }

    private void PlayFire1Animation() 
    {
        float rps = weapon_stats.rpm / 60;
        player.animation.PlayerAnimator.SetFloat("RPS", rps);
        player.animation.PlayerAnimator.Play("Fire");
    }
}
