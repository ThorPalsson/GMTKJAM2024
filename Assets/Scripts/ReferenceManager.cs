using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
   public static ReferenceManager Instance; 
   public TruckController Truck; 
   public Transform CameraTransform;
   public CameraFollow Camera;

    public Vector3 CameraOriginalPosition; 


   private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        } else 
        {
            Instance = this; 
        }
   }
}

