using System;
using System.Collections.Generic;
using UnityEngine;

public class ParkTrigger : MonoBehaviour
{
    [SerializeField] private Outpost outpost; 
    [SerializeField] private ParkTriggerCargo cargoList;
    [SerializeField] private float parkingSpeed = .2f; 
    private TruckController truck => ReferenceManager.Instance.Truck; 

    private int wheelAmount; 

    private bool isParked;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Wheels"))
        {
            wheelAmount++; 
            print ($"Wheel nr {wheelAmount} added");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("Wheels"))
        {
            wheelAmount --; 
            print ($"Wheel nr {wheelAmount} removed");
        }
    }

    private void StartParking()
    {
           outpost.ParkCar(cargoList.GetCargos());
           isParked = true;
    }


    private void Update()
    {
        if (isParked && wheelAmount != 4)
        {
            isParked = false;
        }

        if (wheelAmount == 4 && !isParked)
        {
            var isStop = truck.Speed < parkingSpeed;

            if (isStop)
            {
                StartParking();
            }
        }
    }
}
