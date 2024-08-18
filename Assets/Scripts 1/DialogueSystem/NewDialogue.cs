using UnityEngine;
using Anchry.Dialogue; 

public class NewDialogue : MonoBehaviour
{
    public DialogueContainer Conversation; 
    [SerializeField] private ActorController character;

    [ContextMenu("Test Dialogue")]
    public void Interact()
    {
        //Dialogue.Instance.StartDialogue(Conversation , character.Character); 
    }
}
