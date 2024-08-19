using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeStore : MonoBehaviour
{
    //public UpgradeItem[] upgrades; 

    [Serializable]
    public struct UiItem
    {
        public TMP_Text nameText;
        public TMP_Text costText;
        public Image itemImage;
        public Button purchaseButton;
        public GameObject NoAfford; 
    }
    public UiItem[] UiItems;

    private GameObject clickSound => ReferenceManager.Instance.UiClickSound; 

    private void OnEnable() {
        RollItems();
    }

    private void RollItems()
    {
        var cash = ReferenceManager.Instance.gameManager.Money;
        var upgrades = ReferenceManager.Instance.gameManager.GetItems();
        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < UiItems.Length; i++)
        {
            var uiItem = UiItems[i]; 

            uiItem.nameText.text = upgrades[i].ItemName; 
            uiItem.costText.text = upgrades[i].ItemCost.ToString() + "$";
            uiItem.itemImage.sprite = upgrades[i].ItemImage;

            uiItem.purchaseButton.onClick.RemoveAllListeners();
            if (cash >= upgrades[i].ItemCost)
            {
                uiItem.NoAfford.SetActive(false);
                uiItem.purchaseButton.interactable = true;
                ConnectUpgradeToButton(uiItem.purchaseButton, upgrades[i].Type, upgrades[i]);
            }else 
            {
                uiItem.NoAfford.SetActive(true); 
                uiItem.purchaseButton.interactable = false;
            }

            uiItem.purchaseButton.onClick.AddListener(() => Instantiate(clickSound));
        }
    }

    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }


    private void ConnectUpgradeToButton(Button b, UpgradeItem.UpgradeType type, UpgradeItem item)
    {
        var truck = ReferenceManager.Instance.Truck; 

        switch(type)
        {
            case UpgradeItem.UpgradeType.EnginePower:
                b.onClick.AddListener(() => TryBuyPower(truck, item));
                break;
            case UpgradeItem.UpgradeType.BrakePower:
                b.onClick.AddListener(() => TryBuyBrakes(truck, item));
                break;
            case UpgradeItem.UpgradeType.Nitro:
                b.onClick.AddListener(() => TryBuyBoost(truck, item));
                break;
            case UpgradeItem.UpgradeType.Gearbox:
                b.onClick.AddListener(() => TryBuyGearBox(truck, item));
                break;
            case UpgradeItem.UpgradeType.BackBar:
                b.onClick.AddListener(() => TryBuyBackBar(truck, item));
                break;
        }

        //b.onClick.AddListener(() => RollItems());

    }


    private void TryBuyPower(TruckController truck, UpgradeItem item)
    {
        print ("Trying to buy power");
        if (!TryPurchase(item)) return;
        truck.AddPower();
        RollItems();

    }

    private void TryBuyBrakes(TruckController truck, UpgradeItem item)
    {
        if (!TryPurchase(item)) return;
        truck.AddBreaks();
        RollItems();
    }

    private void TryBuyBoost(TruckController truck, UpgradeItem item)
    {
        if (!TryPurchase(item)) return; 
        truck.NitroUpgrade();
        RollItems();
    }

    private void TryBuyGearBox(TruckController truck, UpgradeItem item)
    {
        if (!TryPurchase(item)) return;
        truck.UpdateGearBox();
        RollItems();
    }

    private void TryBuyBackBar(TruckController truck, UpgradeItem item)
    {
        if (!TryPurchase(item)) return; 
        truck.AddBackBar();
        RollItems();
    }


    private bool TryPurchase(UpgradeItem item)
    {
        var cash = ReferenceManager.Instance.gameManager.Money; 
        if(cash >= item.ItemCost)
        {
            ReferenceManager.Instance.gameManager.RemoveMoney(item.ItemCost);
            float f = item.ItemCost; 
            item.ItemCost = Mathf.RoundToInt(f *= 1.2f);   
            item.Used = true; 
            return true;
        }

        print ("Too pooor!"); 
        return false;
    }
}
