using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Game.Weapon;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;



namespace Game.Weapon 
{
    [Serializable]
    public struct BulletStats
    {
        public float Damage;
        public float Penatration;
        public float Range;
        public float Speed; //715 m/s muzzle velocity of AK-47
        public float DamageFalloff;
        public float HeightFalloff;

        public BulletStats(float damage, float penetration, float range, float speed, float damagefalloff, float heightfalloff)
        {
            Damage = damage;
            Penatration = penetration;
            Range = range;
            Speed = speed;
            DamageFalloff = damagefalloff;
            HeightFalloff = heightfalloff;            
        }
    }

}

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletStats _bullet;

    private Vector3 start_pos;
    private bool initalized = false;
    private float fallAngle = 0f;
    private float travel_distance = 0f;
    private List<RaycastHit> hitList;
    private RaycastHit previousHit;

    public void Initialize(BulletStats bullet)
    {
        this._bullet = bullet;
        start_pos = transform.position;
        initalized = true;
    }
    public void Initialize(float damage, float penetration, float range, float speed, float damagefalloff, float heightfalloff)
    {
        _bullet.Damage = damage;
        _bullet.Penatration = penetration;
        _bullet.Range = range;
        _bullet.Speed = speed;
        _bullet.DamageFalloff = damagefalloff;
        _bullet.HeightFalloff = heightfalloff;

        start_pos = transform.position;
        initalized = true;
        hitList = new List<RaycastHit>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!initalized) return;

        RaycastHit[] hits;
        float dist = _bullet.Speed * Time.deltaTime;

        if (dist + travel_distance > _bullet.Range) //if the staince will take us past our max range set this distance to take us to our max_range
        {
            dist = (_bullet.Range - travel_distance) + 0.05f; //adding 0.05f incase of floating point errors when checking equality of travel_dist and _bullet.range
                                                              //Instead can just check if travel dist is greater
        }      

        hits = Physics.RaycastAll(transform.position, transform.forward, dist); //Cast ray and see what it hits
        if(hits.Length <= 0) 
        {
            Vector3 end_pos = transform.forward * dist;
            Debug.DrawRay(transform.position, end_pos, Color.green);
            travel_distance += dist;
            _bullet.Damage -= _bullet.DamageFalloff * dist;            
            if (dist >= _bullet.Range) 
            {
                Destroy(gameObject);
                return;
            }
            transform.position = end_pos;
            return;
        }

        
        
        Debug.DrawRay(transform.position, hits[0].point, Color.green); //show green ray to show bullets travel to the collider

        foreach (RaycastHit hit in hits) 
        {
            hitList.Add(hit);
            if (hitList.Count == 2)
            {
                float pen_dist = Vector3.Distance(hitList[0].point, hitList[1].point);

                //get the density of the material it passed through.
                PenetrationMaterial pen_mat = hitList[0].collider.gameObject.GetComponent<PenetrationMaterial>();
                float density = 1f; //creating default value for density
                if (pen_mat != null) { density = pen_mat.Density; } //assigning value of pen_mat to density if penmat isnt null

                //calculate bullet damage
                float dmg = (pen_dist * density) / _bullet.Penatration;
                if (dmg > _bullet.Damage) dmg = _bullet.Damage; //capping dmg at the remaining damage left in the bullet
                //---------Apply damage to character------ TBD
                //Reduce bullets damage
                _bullet.Damage -= dmg;

                if (Mathf.Abs(_bullet.Damage) <= 0.05f)
                {
                    Debug.DrawRay(hitList[0].point, hitList[1].point, Color.red); //Draw red ray to represent the bullet beign destoyed in the collider
                    travel_distance += Vector3.Distance(transform.position, hits[0].point);
                    Destroy(gameObject); //finaliziung collision
                    return;
                }
                else
                {
                    Debug.DrawRay(hitList[0].point, hitList[1].point, Color.cyan); //draw cyan ray to show the bullet has passed through collder
                    previousHit = hitList[1];
                }
            }
            else 
            {
                if (previousHit.collider != null) 
                {
                    Debug.DrawRay(previousHit.point, hitList[0].point, Color.green); //draw green ray from previous hit to current first hit
                   
                }
            }                    
        }
        
        int index = 0;
        while(index < hits.Length) 
        {
            if (index < (hits.Length - 1)) //If there are at least 2 hits left
            {
                //Calculate distance between impact and exit
                float pen_dist = Vector3.Distance(hits[index].point, hits[index + 1].point);
                if (pen_dist == 0) 
                {
                    Debug.Log("Penetration distance of 0?");
                    return;
                }

                

              


            }
        }
        

    }

    private void FinalizeImpact(RaycastHit hit) 
    {
    }
}
