using UnityEngine;
using Anchry.Dialogue; 

public class NewDialogue : MonoBehaviour
{
    public DialogueContainer Conversation; 
    [SerializeField] private GameObject dialougeCanvas; 

    [ContextMenu("Test Dialogue")]
    public void Interact()
    {
        dialougeCanvas.SetActive(true); 
        Dialogue.Instance.StartDialogue(Conversation); 
    }
}
