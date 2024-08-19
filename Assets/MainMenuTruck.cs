using UnityEngine;

public class MainMenuTruck : MonoBehaviour
{
    public WheelCollider[] wheels; 
    public float power = 500; 


    private void LateUpdate() 
    {
        for(int i= 0; i < wheels.Length; i++)
        {
            wheels[i].motorTorque = power; 
        }    
    }
}
