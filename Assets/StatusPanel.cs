using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    public string[] Quotes; 
    public TMP_Text MoneyText; 
    public TMP_Text LifeTimeWeight; 
    public TMP_Text LifeTimeMoney; 
    public TMP_Text QuoteText;
    [SerializeField] private GameObject panel; 
    [SerializeField] private Button exitButton; 



    public void ShowPanel(float money, Outpost outpost)
    {
        var gameManager = ReferenceManager.Instance.gameManager;
        MoneyText.text = $"Your brought {money}$ worth of cargo"; 
        LifeTimeMoney.text = $"{gameManager.LifeTimeMoney}$";
        LifeTimeWeight.text = $"{gameManager.AllGatheredMass}kg"; 
        QuoteText.text = Quotes[Random.Range(0, Quotes.Length)];

        panel.SetActive(true);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => outpost.ToggleUI(true));
        exitButton.onClick.AddListener(() => panel.SetActive(false));
    }
}
