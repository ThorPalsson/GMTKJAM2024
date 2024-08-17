using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public int Money; 
   public UpgradeItem[] Upgrades;

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


   public UpgradeItem[] GetItems()
   {
        var list = Upgrades.ToList();

        while(list.Count > 3)
        {
            list.Remove(list[UnityEngine.Random.Range(0, list.Count)]); 
        }

        return list.ToArray();
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
    }

    public UpgradeType Type;
}
