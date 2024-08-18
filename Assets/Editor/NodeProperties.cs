using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeProperties : Node
{
    public string GUID;
    public bool EntryPoint = false;

    //GUID + enum type
    public Dictionary<string,string> LinkIDs = new Dictionary<string, string>();

    public enum LinkTypes   
    {
        NONE,
        NEED_ITEM,
        NEED_QUEST,
        GIVE_QUEST,
        GIVE_ITEM,
        GIVE_ATTITUDE
    }

    public LinkTypes ThisLink;
}
