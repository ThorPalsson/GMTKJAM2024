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

            ConnectUpgradeToButton(uiItem.purchaseButton, upgrades[i].Type);
        }
    }

    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }


    private void ConnectUpgradeToButton(Button b, UpgradeItem.UpgradeType type)
    {
        var truck = ReferenceManager.Instance.Truck; 
        switch(type)
        {
            case UpgradeItem.UpgradeType.EnginePower:
                b.onClick.AddListener(() => truck.AddPower());
                break;
            case UpgradeItem.UpgradeType.BrakePower:
                b.onClick.AddListener(() => truck.AddBreaks());
                break;
            case UpgradeItem.UpgradeType.Nitro:
                b.onClick.AddListener(() => truck.NitroUpgrade());
                break;
        }

        b.onClick.AddListener(() => ClosePanel());

    }
}
