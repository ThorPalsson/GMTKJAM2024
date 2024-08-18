using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool HasFallen; 

    [SerializeField] private GameObject fallenEffect;
    public int cargoValue = 1;

    [SerializeField] private bool useFallEffect = true;
    [SerializeField] private bool killParent = false;
    [SerializeField] private GameObject parent; 


    private void OnCollisionEnter(Collision other) {
        if (!HasFallen) return;

        if (other.transform.CompareTag("Car") || other.transform.CompareTag("Cargo"))
            return; 

        DestroyCargo(false);
    }


    public void DestroyCargo(bool money = false)
    {
        if (useFallEffect)
            Instantiate(fallenEffect, transform.position, Quaternion.identity);
        
        if (killParent)
        {
            Destroy(parent); 
        }

        Destroy(this.gameObject);
    }
}
