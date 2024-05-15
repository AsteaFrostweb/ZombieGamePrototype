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

    [Header("Outputs")]
    [SerializeField]
    private bool PositionAligned;
    [SerializeField]
    private bool RotationAligned;
    [SerializeField]
    private CameraPosition TrackedPosition;
  
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
    void Update()
    {
        if (TrackedPosition.transform == null) return;
        if (MainCamera == null) return;

        PositionAligned = false;
        RotationAligned = false;

        float distance = (MainCamera.transform.position - TrackedPosition.transform.position).magnitude;       

        if (distance < 0.05f)
        {
           MainCamera.transform.position = TrackedPosition.transform.position;
            PositionAligned = true;
        }
        if (Vector3.Angle(MainCamera.transform.rotation.eulerAngles.normalized, TrackedPosition.transform.rotation.eulerAngles.normalized) < 0.1f)
        {
           MainCamera.transform.rotation = TrackedPosition.transform.rotation;
            RotationAligned = true;
        }

        if (!PositionAligned)
        {
           MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, TrackedPosition.transform.position, MoveSpeed * Time.deltaTime);
        }

        if (!RotationAligned) 
        {
           MainCamera.transform.rotation = Quaternion.RotateTowards(MainCamera.transform.rotation, TrackedPosition.transform.rotation, RotationSpeed);
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
