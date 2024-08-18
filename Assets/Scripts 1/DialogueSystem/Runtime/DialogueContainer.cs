using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anchry.Dialogue
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        public List<AttributeNodeData> AttributeNodeDatas = new List<AttributeNodeData>();
        public List<TraitNodeData> TraitNodeDatas = new List<TraitNodeData>();
        public List<StoryNodeData> StoryNodeDatas = new List<StoryNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
        public List<ItemNodeData> ItemNodeDatas = new List<ItemNodeData>();
        public List<MovementNodeData> MovementNodeDatas = new List<MovementNodeData>();
        public List<AnimationNodeData> AnimationNodeDatas = new List<AnimationNodeData>();
        public List<FocusNodeData> FocusNodeDatas = new List<FocusNodeData>();
        public List<ContinueNodeData> ContinueNodeDatas = new List<ContinueNodeData>();
    }
}
