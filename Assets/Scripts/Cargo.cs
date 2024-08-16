using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool HasFallen; 

    [SerializeField] private GameObject fallenEffect;


    private void OnCollisionEnter(Collision other) {
        if (!HasFallen) return;

        if (other.transform.CompareTag("Car") || other.transform.CompareTag("Cargo"))
            return; 

        Instantiate(fallenEffect, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
