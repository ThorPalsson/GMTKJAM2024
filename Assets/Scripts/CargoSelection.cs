using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CargoSelection : MonoBehaviour
{

    [Serializable]
    public struct UiItem
    {
        public TMP_Text nameText;
        public TMP_Text estimatedCost;
        public Image itemImage;
        public Button selectionButton;
    }

    public UiItem[] uiItems; 

    private TruckController trucK;

    private GameObject clickSound => ReferenceManager.Instance.UiClickSound; 

    public void Start()
    {
        trucK = ReferenceManager.Instance.Truck; 
    }

    private void OnEnable() 
    {
        if (trucK == null)
        {
            trucK = ReferenceManager.Instance.Truck; 
        }

        RollItems();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RollItems()
    {
        var items = ReferenceManager.Instance.gameManager.GetCargo();

        for (int i = 0; i < uiItems.Length; i++)
        {
            var uiItem = uiItems[i]; 

            uiItem.nameText.text = items[i].CargoName; 
            uiItem.estimatedCost.text = $"Estimated Cost: <br> <size=28px>{items[i].CargoWorth}";
            uiItem.itemImage.sprite = items[i].CargoImage;

            uiItem.selectionButton.onClick.RemoveAllListeners();

            ConnectSelectionButton(uiItem.selectionButton, items[i].CargoObject);
            uiItem.selectionButton.onClick.AddListener(() => Instantiate(clickSound)); 
        }
    }

    private void ConnectSelectionButton(Button b, GameObject cargo)
    {
        b.onClick.AddListener(() => trucK.AddCargo(cargo)); 
        b.onClick.AddListener(() => ClosePanel(cargo));
    }

    private void ClosePanel(GameObject g)
    {
        Time.timeScale = 1;
        ReferenceManager.Instance.gameManager.StoreCargo(g);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameObject.SetActive(false);
    }
}
