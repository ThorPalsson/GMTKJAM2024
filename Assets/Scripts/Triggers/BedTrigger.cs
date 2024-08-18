using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask discarderLayer;
    [SerializeField] private Transform CargoParent; 


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cargo"))
        {
            if (other.transform.parent == null)
            {
                other.transform.parent = CargoParent;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Cargo"))
        {
            var cargo = other.gameObject; 
           // cargo.layer = discarderLayer; 
            cargo.GetComponent<Cargo>().HasFallen = true;
            cargo.transform.parent = null;
        }
    }
}
