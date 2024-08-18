using System;
using UnityEngine;

[Serializable]
public class ItemNode : NodeProperties
{
    public int ItemID;

    public enum NodeType
    {
        GiveItem,
        NeedsItem,
    };

    public NodeType Type;

    public int NodeTypeId;

    public void Translate(string name)
    {
        switch (name)
        {
            case "GiveItem":
                Type = NodeType.GiveItem;
                NodeTypeId = 0;
                break;
            case "NeedsItem":
                Type = NodeType.NeedsItem;
                NodeTypeId = 1;
                break;
        }
    }

    public void Translate(int index)
    {
        Type = (NodeType) index;
        NodeTypeId = index;
    }
}
