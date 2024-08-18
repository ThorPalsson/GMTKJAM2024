using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anchry.Dialogue;
using UnityEditor.Experimental.GraphView;

public class ContinueNodeData : NodeData
{
    public string DialogueText;
    public bool IsEndNode; 
    public int CharacterID; 
}
