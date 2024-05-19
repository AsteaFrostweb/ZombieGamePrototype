using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPlacer : MonoBehaviour
{
    public GameObject PlacedObject;
    public float RayLength;
    public Vector3 RayLocalDirection;

    public float MinimumDistance = 5f;

    private Vector3 rayDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rayDirection = transform.TransformDirection(RayLocalDirection);

        rayDirection = rayDirection.normalized;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, rayDirection, out hit, RayLength)) 
        {
            if (hit.distance >= MinimumDistance)
            {
                PlacedObject.transform.position = hit.point;
            }
        }
    }
}
