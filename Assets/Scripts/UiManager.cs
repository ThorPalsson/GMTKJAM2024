using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text MoneyText;

    public void ChangeMoneyText(int money)
    {
        MoneyText.text = money.ToString();
    }
}
