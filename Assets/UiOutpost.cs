using UnityEngine;
using UnityEngine.UI;

public class UiOutpost : MonoBehaviour
{
    [SerializeField] private Button[] buttons; 
    void Start()
    {
        var clickSound = ReferenceManager.Instance.UiClickSound; 
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => Instantiate(clickSound)); 
        }
    }


}
