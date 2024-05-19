using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CameraTweener : MonoBehaviour
{
    [Serializable]
    public struct CameraPosition 
    {
        public Transform transform;
        public string name;
        public int id;     
    }

    [Header("Inputs")]
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private List<CameraPosition> CameraPositions;
    [SerializeField]
    [Range(0.001f, 100f)]
    public float MoveSpeed;
    [SerializeField]
    [Range(0.001f, 100f)]
    public float RotationSpeed;
    [SerializeField]
    private float clippingOffset = 0.05f;
    [SerializeField]
    private LayerMask Layer_Mask;
 

    [Header("Outputs")]
    [SerializeField]
    private bool PositionAligned;
    [SerializeField]
    private bool RotationAligned;
    [SerializeField]
    private bool isClipping;
    [SerializeField]
    private CameraPosition TrackedPosition;


    private Vector3 tracked_pos;
  
  
    // Start is called before the first frame update
    void Start()
    {
        if (CameraPositions.Count < 1) 
        {
            Debugging.Log("No camera positons set in camera tweener. Unable to asign default position");
            return;
        }
        TrackedPosition = CameraPositions[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TrackedPosition.transform == null) return;
        if (MainCamera == null) return;
    


        RaycastHit rayHit = new RaycastHit();

        Vector3 toPos = (TrackedPosition.transform.position - transform.position);
        Vector3 dir = toPos.normalized;
        float len = toPos.magnitude;

        if (Physics.Raycast(transform.position, dir, out rayHit, len +  clippingOffset, Layer_Mask))
        {            
            //Debug.DrawRay(transform.position, rayHit.point, Color.green);
            isClipping = true;
        }
        else 
        {
            isClipping = false;
            //Debug.DrawRay(transform.position, rayHit.point, Color.red);
        }

        if (isClipping)
        {
            tracked_pos = rayHit.point - (dir * clippingOffset);
        }
        else tracked_pos = TrackedPosition.transform.position;


        CheckAllignment();

        if (!PositionAligned)
        {
            float lerp_factor = isClipping ? 1.0f : MoveSpeed * Time.deltaTime;
            lerp_factor = (rayHit.distance > Vector3.Distance(transform.position, MainCamera.transform.position)) ? MoveSpeed * Time.deltaTime : lerp_factor;
           MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, tracked_pos, lerp_factor);
        }

        if (!RotationAligned) 
        {
           MainCamera.transform.rotation = Quaternion.RotateTowards(MainCamera.transform.rotation, TrackedPosition.transform.rotation, RotationSpeed);
        }

    }

    private void CheckAllignment() 
    {
        PositionAligned = false;
        RotationAligned = false;

        float distance = (MainCamera.transform.position - tracked_pos).magnitude;
        if (distance < 0.05f)
        {
            MainCamera.transform.position = tracked_pos;
            PositionAligned = true;
        }
        if (Vector3.Angle(MainCamera.transform.rotation.eulerAngles.normalized, TrackedPosition.transform.rotation.eulerAngles.normalized) < 0.1f)
        {
            MainCamera.transform.rotation = TrackedPosition.transform.rotation;
            RotationAligned = true;
        }

    }

    public void SetTrackedTransform(string name) 
    {
        foreach (CameraPosition cp in CameraPositions)
        {
            if (cp.name == name)
            {
                TrackedPosition = cp;
            }
        }
    }
    public void SetTrackedTransform(Transform trans)
    {
        foreach (CameraPosition cp in CameraPositions)
        {
            if (cp.transform == trans)
            {
                TrackedPosition = cp;
            }
        }
    }
    public void SetTrackedTransform(int id)
    {
        foreach (CameraPosition cp in CameraPositions) 
        {
            if (cp.id == id) 
            {
                TrackedPosition = cp;
            } 
        }
    }
}
