using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private GameObject truck; 
    [SerializeField] private TMP_Text TitleText; 

    private void Start() {
        playButton.onClick.AddListener(() => Play()); 
    }

    public void Play()
    {
        TitleText.text = "CARGO TRUCK!!!";
        playButton.gameObject.SetActive(false); 
        truck.SetActive(true); 
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(12);

        SceneManager.LoadScene("MainScene");

    }
}
