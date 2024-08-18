using UnityEngine;
using Anchry.Dialogue; 

public class NewDialogue : MonoBehaviour
{
    public DialogueContainer Conversation; 

    public void StartDialogue()
    {
        ReferenceManager.Instance.DialougeCanvas.SetActive(true); 
        ReferenceManager.Instance.DialogueSystem.StartDialogue(Conversation);
    }
}
