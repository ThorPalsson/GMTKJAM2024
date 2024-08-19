using UnityEngine;
using Anchry.Dialogue; 

public class NewDialogue : MonoBehaviour
{
    public DialogueContainer Conversation; 
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Outpost outpost; 
    [SerializeField] private bool EndTheGame;

    public void StartDialogue()
    {
        outpost.StartDialogue(cameraPosition);
        ReferenceManager.Instance.DialougeCanvas.SetActive(true); 
        ReferenceManager.Instance.DialogueSystem.StartDialogue(Conversation, outpost);
    }
}
