using TMPro;
using UnityEngine;

public class OutpostName : MonoBehaviour
{
    public Outpost outpost;
    public TMP_Text text;
    public float timer;

    void Start()
    {
        text.text = outpost.OutpostName; 
        Destroy(this.gameObject, timer); 
    }

}
