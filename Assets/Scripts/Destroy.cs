using UnityEngine;

public class Destroy : MonoBehaviour
{

    [SerializeField] private float time = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, time);
    }
}
