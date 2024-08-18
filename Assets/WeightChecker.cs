using TMPro;
using UnityEngine;

public class WeightChecker : MonoBehaviour
{
    public int Weight; 
    [SerializeField] private TMP_Text WeightText; 

    private void Update() {
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();

        float floatingWeight = 0; 

        for (int i = 0; i < rb.Length; i++)
        {
            floatingWeight += rb[i].mass;
        }

        Weight = Mathf.CeilToInt(floatingWeight); 
        WeightText.text = Weight.ToString()+ "kg"; 
    }
}
