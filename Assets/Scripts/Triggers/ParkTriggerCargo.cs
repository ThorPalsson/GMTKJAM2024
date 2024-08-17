using System.Collections.Generic;
using UnityEngine;

public class ParkTriggerCargo : MonoBehaviour
{
   private List<GameObject> cargo = new List<GameObject>();


   public Cargo[] GetCargos()
   {
        var cargoList = new List<Cargo>();

        foreach (var c in cargo)
        {
            if (c == null) continue;
            var newCargo = c.GetComponent<Cargo>();
            cargoList.Add(newCargo);
        }

        cargo.Clear();
        return cargoList.ToArray();

   }


   private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Cargo")) return;

        cargo.Add(other.gameObject);
   }


   private void OnTriggerExit(Collider other) {

        if (!other.CompareTag("Cargo")) return;

        if (cargo.Contains(other.gameObject))
        {
            cargo.Remove(other.gameObject);
        }

   }
}
