using System.Runtime.InteropServices;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        
    public Vector3 offset;          
    public float smoothSpeed = 0.125f; 

    public bool FollowCar = true;

    void LateUpdate()
    {
        if (!FollowCar) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}