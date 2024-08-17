using System.Runtime.InteropServices;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        
    public Vector3 offset;          
    public float smoothSpeed = 0.125f; 

    public bool FollowCar = true;

    public Vector3 Position; 

    private float yOffsetLimit = .3f;   

    void FixedUpdate()
    {

        Position = transform.position;
        if (!FollowCar) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        if (Mathf.Abs(desiredPosition.y - transform.position.y ) < yOffsetLimit)
        {
            desiredPosition = new Vector3(desiredPosition.x, transform.position.y, desiredPosition.z);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}