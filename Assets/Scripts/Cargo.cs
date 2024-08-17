using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool HasFallen; 

    [SerializeField] private GameObject fallenEffect;
    public int cargoValue = 1;


    private void OnCollisionEnter(Collision other) {
        if (!HasFallen) return;

        if (other.transform.CompareTag("Car") || other.transform.CompareTag("Cargo"))
            return; 

        DestroyCargo(false);
    }


    public void DestroyCargo(bool money = false)
    {
        Instantiate(fallenEffect, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
