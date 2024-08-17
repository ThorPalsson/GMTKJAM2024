using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public int Money; 
   private TruckController truck => ReferenceManager.Instance.Truck; 
   public UpgradeItem[] Upgrades;
   public CargoTypes[] CargoTypes;

   private Vector3 savePosition; 
   private Quaternion saveRotation;
   [SerializeField] private GameObject savedCargo; 

   public void AddMoney(int value)
   {
        Money += value;
        ReferenceManager.Instance.uiManager.ChangeMoneyText(Money);
   }

   public void RemoveMoney(int value)
   {
        Money -= value; 
        ReferenceManager.Instance.uiManager.ChangeMoneyText(Money);
   }

    private void Start()
    {
        savePosition = truck.transform.position;
    }

   private void Update()
   {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTruck();
        }
   }


   public UpgradeItem[] GetItems()
   {
        var list = Upgrades.ToList();
        ShuffleList(list); 
        var returnList = new List<UpgradeItem>();

        for(int i = 0; i < 3; i++)
        {
            returnList.Add(list[i]); 
        }

        return list.ToArray();
   }

    public CargoTypes[] GetCargo()
    {
        var list = CargoTypes.ToList();
        ShuffleList(list);
        var returnList = new List<CargoTypes>();

        for(int i = 0; i < 3; i++)
        {
            returnList.Add(list[i]); 
        }

        return list.ToArray();
   }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

   public void StoreTruck(GameObject cargo)
   {
        savePosition = truck.transform.position;
        saveRotation = truck.transform.rotation; 
        savedCargo = cargo; 
   }

   public void ResetTruck()
   {
        truck.ComingFromRespawn = true;
        truck.transform.position = savePosition; 
        truck.transform.rotation = saveRotation; 
        truck.rb.linearVelocity = Vector3.zero;
        truck.rb.angularVelocity = Vector3.zero; 

        if (savedCargo != null)
            truck.AddCargo(savedCargo);
   }
   
}

[Serializable]
public class UpgradeItem 
{
    public string ItemName;
    public int ItemCost;
    public Sprite ItemImage; 


    public enum UpgradeType
    {
        EnginePower,
        BrakePower,
        Nitro, 
        Gearbox,
    }

    public UpgradeType Type;
}


[Serializable]
public class CargoTypes
{
    public string CargoName; 
    public int CargoWorth; 
    public Sprite CargoImage; 
    public GameObject CargoObject; 
}
