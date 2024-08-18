using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anchry.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ColorUtility = UnityEngine.ColorUtility;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance; 
    public GameObject ConversationParent;
    public Transform TextContainer;
    public GameObject AnswerButton;

    private int _textStoreAmount = 15;
    private List<GameObject> ActiveText = new List<GameObject>();
    private List<GameObject> ActiveAnswers = new List<GameObject>();
    

    public CharacterContainer characters;
    
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text talkerText;
    private DialogueContainer _currentDialogue;
    private Outpost outpost; 
    [SerializeField] private DialogueContainer IntroDialouge; 
    private bool isIntro = true; 

    [SerializeField] private GameObject truck, truckCamera, introCamera; 
    
    //public bool ActiveDialogue => ConversationParent.activeSelf; 

    [Header("Debug")]
    public DialogueContainer TestContainer;

    public enum NodeTypes
    {
        DialougeNode, 
        EndNode,
    }

	private void Awake() 
	{
		if (Instance != null && Instance != this)
			Destroy(this); 
		else 
			Instance = this; 	
	}

    private void Start()
    {
        StartDialogue(IntroDialouge, null); 
    }

    public void StartDialogue(DialogueContainer dialogue, Outpost post)
    {
        outpost = post;
        _currentDialogue = dialogue; 
        NextDialogue(_currentDialogue.NodeLinks[0].TargetNodeGUID);  
    }

    [ContextMenu("Tesst")]
    private void StartTestDialogue()
    {  
        _currentDialogue = TestContainer; 
        //ConversationParent.SetActive(true); 
        NextDialogue(_currentDialogue.NodeLinks[0].TargetNodeGUID); 
    }

    private void NextDialogue(string Guid)
    {
        NodeTypes node = GetNodeType(Guid); 

        switch(node)
        {
            case NodeTypes.DialougeNode:
                SetDialogue(Guid); 
                break; 
            case NodeTypes.EndNode:
                EndDialogue();
                break;
        }
    }
    
    private void SetDialogue(string Guid)
    {
        DialogueNodeData nodeData = _currentDialogue.DialogueNodeDatas.FirstOrDefault(t => t.NodeGUID == Guid);

        if (!HasAnswers(Guid))
        {
            EndDialogue();
            return; 
        }

        foreach (var ans in ActiveAnswers)
        {
            Destroy(ans);
        }
        ActiveAnswers.Clear();


        dialogueText.text = SetUpDialogueUI(nodeData);
        StartCoroutine(SetAnswers(Guid)); 

        //NodeLinkData[] answerArray = _currentDialogue.NodeLinks.Where(t => t.BaseNodeGUID == Guid).ToArray();
       // StartCoroutine(WaitAndNextMessage(3f, answerArray[0]));
    }

    private IEnumerator WaitAndNextMessage(float wait, NodeLinkData node)
    {
        yield return new WaitForSeconds(wait);
        NextDialogue(node.TargetNodeGUID);
        
    }

    private NodeTypes GetNodeType(string guid)
    {
        if (_currentDialogue.DialogueNodeDatas.FirstOrDefault(x => x.NodeGUID == guid) != default)
            return NodeTypes.DialougeNode;

        if (_currentDialogue.EndNodeDatas.FirstOrDefault(x => x.NodeGUID == guid) != default)
            return NodeTypes.EndNode;
        
        Debug.LogError("[Dialogue] couldn't find GUID exiting dialogue");
        EndDialogue();

        return NodeTypes.DialougeNode; 
    }

    private bool HasAnswers(string guid)
    {
        foreach(var node in _currentDialogue.NodeLinks)
        {
            if (node.BaseNodeGUID == guid)
                return true;
        }

        print ("Found no answers for the node"); 
        return false; 
    }

    public void EndDialogue()
    {
        for(int i = 0; i < ActiveAnswers.Count; i++)
            Destroy(ActiveAnswers[i].gameObject);

        ActiveAnswers.Clear();

        if (ActiveText.Count > _textStoreAmount)
        {
            int textToRemove = ActiveText.Count - _textStoreAmount;
            ActiveText.RemoveRange(0, textToRemove);
        }

        if (!isIntro)
            outpost.EndDialogue();
        else 
        {
            isIntro = false;
            truck.SetActive(true); 
            introCamera.SetActive(false);
            truckCamera.SetActive(true);
        }
        

        print ("Ending Dialogue");
        dialogueText.text = "";
        //ConversationParent.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private string SetUpDialogueUI(DialogueNodeData nodeData)
    {
        print($"Character id = {nodeData.CharacterID}");
        
        string talker = characters.Characters[nodeData.CharacterID].CharacterName;
        string color = ColorUtility.ToHtmlStringRGB(characters.Characters[nodeData.CharacterID].CharacterUiColor);
        
        var charName = $"<color=#{color}><b>{talker}</b></color> - ";
        talkerText.text = charName;

        var dialogueText = nodeData.DialogueText;
        return dialogueText;
    }

    private IEnumerator SetAnswers(string Guid)
    {
        yield return new WaitForSeconds(.2f);
        
        for(int i = 0; i < ActiveAnswers.Count; i++)
        {
            if (ActiveAnswers[i] == null) continue; 
            Destroy(ActiveAnswers[i].gameObject); 
        }

        ActiveAnswers.Clear();
        NodeLinkData[] answerArray = _currentDialogue.NodeLinks.Where(t => t.BaseNodeGUID == Guid).ToArray();

        foreach(var answer in answerArray)
        { 
            string message = answer.PortName;
            Button answerButton = Instantiate(AnswerButton, transform.position, Quaternion.identity, TextContainer).GetComponent<Button>();
            answerButton.GetComponentInChildren<TMP_Text>().text = answer.PortName; 
            answerButton.onClick.AddListener(delegate{NextDialogue(answer.TargetNodeGUID);});  
            ActiveAnswers.Add(answerButton.gameObject); 
        }

        if (ActiveAnswers.Count == 0)
        {
            Debug.LogError("Error, Dialogue had no reachable answers. Leaving dialogue");
            EndDialogue();
        }
    }
}
