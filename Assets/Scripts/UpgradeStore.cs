using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStore : MonoBehaviour
{
    public UpgradeItem[] upgrades; 

    [Serializable]
    public struct UiItem
    {
        public TMP_Text nameText;
        public TMP_Text costText;
        public Image itemImage;
        public Button purchaseButton;
    }
    public UiItem[] UiItems;


    private void Start()
    {
        upgrades = ReferenceManager.Instance.gameManager.GetItems();


        for (int i = 0; i < upgrades.Length; i++)
        {
            var uiItem = UiItems[i]; 

            uiItem.nameText.text = upgrades[i].ItemName; 
            uiItem.costText.text = upgrades[i].ItemCost.ToString() + "$";
            uiItem.itemImage.sprite = upgrades[i].ItemImage;

            ConnectUpgradeToButton(uiItem.purchaseButton, upgrades[i].Type, upgrades[i].ItemCost);
        }
    }

    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }


    private void ConnectUpgradeToButton(Button b, UpgradeItem.UpgradeType type, int price)
    {
        var truck = ReferenceManager.Instance.Truck; 


        switch(type)
        {
            case UpgradeItem.UpgradeType.EnginePower:
                b.onClick.AddListener(() => TryBuyPower(truck, price));
                break;
            case UpgradeItem.UpgradeType.BrakePower:
                b.onClick.AddListener(() => TryBuyBrakes(truck, price));
                break;
            case UpgradeItem.UpgradeType.Nitro:
                b.onClick.AddListener(() => TryBuyBoost(truck, price));
                break;
        }

        b.onClick.AddListener(() => ClosePanel());
    }


    private void TryBuyPower(TruckController truck, int price)
    {
        if (!TryPurchase(price)) return;
        truck.AddPower();
    }

    private void TryBuyBrakes(TruckController truck, int price)
    {
        if (!TryPurchase(price)) return;
        truck.AddBreaks();
    }

    private void TryBuyBoost(TruckController truck, int price)
    {
        if (!TryPurchase(price)) return; 
        truck.NitroUpgrade();
    }


    private bool TryPurchase(int price)
    {
        var cash = ReferenceManager.Instance.gameManager.Money; 
        if(cash >= price)
        {
            ReferenceManager.Instance.gameManager.RemoveMoney(price);
            return true;
        }


        print ("Too pooor!"); 
        return false;



    }
}
