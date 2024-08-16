using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask discarderLayer;

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Cargo"))
        {
            var cargo = other.gameObject; 
            cargo.layer = discarderLayer; 
            cargo.GetComponent<Cargo>().HasFallen = true;
        }
    }
}
