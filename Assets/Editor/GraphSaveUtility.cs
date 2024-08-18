using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
# if UNITY_EDITOR 
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Anchry.Dialogue
{
    public class GraphSaveUtility 
    {
# if UNITY_EDITOR 

        private DialogueGraphView _targetGraphView; 
        private DialogueContainer _containerCache; 

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => GetDialogueNodes();
        private List<AttributeNode> AttributeNodes => GetAttributeNodes();
        private List<TraitNode> TraitNodes => GetTraitNodes();
        private List<EndNode> EndNodes => GetEndNodes();
        private List<StoryNode> StoryNodes => GetStoryNodes();
        private List<ItemNode> ItemNodes => GetItemNodes();
        private List<MovementNode> MovementNodes => GetMovementNodes();
        private List<FocusNode> FocusNodes => GetFocusNodes();
        private List<AnimationNode> AnimationNodes => GetAnimationNodes();

        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {

            return new GraphSaveUtility
            {
                _targetGraphView = targetGraphView
            }; 
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            SaveExposedProperties();
            if(!SaveNodes(dialogueContainer)) return;

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
                AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");

            
            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{fileName}.asset"); 
            AssetDatabase.SaveAssets();        
        }

        private void SaveExposedProperties()
        {
            ExposedPropertyContainer propertyContainer = Resources.Load<ExposedPropertyContainer>("Containers/PropertyContainer");
            propertyContainer.ExposedProperties.Clear();
            propertyContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
        }

        private bool SaveNodes(DialogueContainer container)
        {
            if (!Edges.Any()) return false; 
            
            var ports = Edges.ToArray();

            for (int i = 0; i < ports.Length; i ++ )
            {
                var outputNode = ports[i].output.node as NodeProperties; 
                var inputNode = ports[i].input.node as NodeProperties;  

                container.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID, 
                    PortName = ports[i].output.portName, 
                    TargetNodeGUID = inputNode.GUID
                });
            }  

            foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))            
            {
                container.DialogueNodeDatas.Add(new DialogueNodeData
                {
                    NodeGUID = dialogueNode.GUID,
                    DialogueText = dialogueNode.DialogueText,
                    Position = dialogueNode.GetPosition().position,
                    CharacterID = dialogueNode.CharacterIndex,
                    IsEndNode = dialogueNode.IsEndNode,
                    IsContinue = dialogueNode.IsContinue,
                    ContinueTimer = dialogueNode.ContinueTimer,
                });
            } 

            foreach (var attNode in AttributeNodes.Where(node => !node.EntryPoint))
            {
                container.AttributeNodeDatas.Add
                (
                    new AttributeNodeData
                    {
                        NodeGUID = attNode.GUID,
                        Position = attNode.GetPosition().position,
                        AttributeID = attNode.AttributeID
                    }
                );
            }

            foreach (var traitNode in TraitNodes.Where(node => !node.EntryPoint))
            {
                container.TraitNodeDatas.Add
                (
                    new TraitNodeData
                    {
                        NodeGUID = traitNode.GUID,
                        Position = traitNode.GetPosition().position,
                        TraitID = traitNode.TraitID,
                    }
                );
            }

            foreach (var storyNode in StoryNodes.Where(node => !node.EntryPoint))
            {
                container.StoryNodeDatas.Add(
                    new StoryNodeData
                    {
                        NodeGUID = storyNode.GUID,
                        Position =  storyNode.GetPosition().position,
                        Id = storyNode.StoryPointID,
                        Value = storyNode.StoryString,
                    });   
            }

            foreach (var endNode in EndNodes.Where(node => !node.EntryPoint))
            {
                container.EndNodeDatas.Add(
                    new EndNodeData
                    {
                        NodeGUID = endNode.GUID,
                        Position = endNode.GetPosition().position,
                    });
            }
            
            foreach (var itemNode in ItemNodes.Where(node => !node.EntryPoint))
            {
                container.ItemNodeDatas.Add(
                    new ItemNodeData
                    {
                        NodeGUID = itemNode.GUID,
                        Position = itemNode.GetPosition().position,
                        ItemId = itemNode.ItemID,
                        NodeTypeId = itemNode.NodeTypeId,
                    });
            }
            
            foreach (var node in MovementNodes.Where(node => !node.EntryPoint))
            {
                container.MovementNodeDatas.Add(
                    new MovementNodeData()
                    {
                        NodeGUID = node.GUID,
                        Position = node.GetPosition().position,
                        MovePos = node.Position,
                    });
            }
            
            foreach (var node in FocusNodes.Where(node => !node.EntryPoint))
            {
                container.FocusNodeDatas.Add(
                    new FocusNodeData()
                    {
                        NodeGUID = node.GUID,
                        Position = node.GetPosition().position,
                        CharacterIndex = node.CharacterIndex,
                    });
            }
            
            foreach (var node in AnimationNodes.Where(node => !node.EntryPoint))
            {
                container.AnimationNodeDatas.Add(
                    new AnimationNodeData()
                    {
                        NodeGUID = node.GUID,
                        Position = node.GetPosition().position,
                        AnimationName = node.AnimationName,
                        Active = node.Active,
                        CharacterIndex = node.CharacterIndex,
                    });
            }

            return true;     
        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>("Dialogues/"+ fileName); 

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("Conversation not found Jonathan", "Please write in the correct name of the dialogue you want to load sir.", "I am a slut"); 
                return; 
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
            CreateExposedProperties();
        }

        private void CreateExposedProperties()
        {
            ExposedPropertyContainer propertyContainer = Resources.Load<ExposedPropertyContainer>("Containers/PropertyContainer");
            _targetGraphView.ClearBlackBoardAndExposedProperties();
            
            foreach(var exposedProperty in propertyContainer.ExposedProperties)
            {
                _targetGraphView.AddPropertyToBlackBoard(exposedProperty); 
            }
        }

        private void ConnectNodes()
        {
             for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }

            for (int i = 0; i < AttributeNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == AttributeNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(AttributeNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }

            for (int i = 0; i < TraitNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == TraitNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(TraitNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }

            for (int i = 0; i < StoryNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == StoryNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(StoryNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }
            
            for (int i = 0; i < ItemNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == ItemNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(ItemNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }
            
            for (int i = 0; i < MovementNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == MovementNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(MovementNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }
            
            for (int i = 0; i < FocusNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == FocusNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(FocusNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }
            
            for (int i = 0; i < AnimationNodes.Count; i++)
            {
                var k = i; 
                var connection = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == AnimationNodes[k].GUID).ToList();
                
                for (int j = 0; j < connection.Count(); j++)
                {
                    var targetNodeGUID = connection[j].TargetNodeGUID; 
                    var targetNode = FindTargetNode(targetNodeGUID); 
                    LinkNodes(AnimationNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]); 

                    SetTargetNodePosition(targetNode, targetNodeGUID);
                }
            }
        }

        private void SetTargetNodePosition(NodeProperties targetNode, string targetNodeGUID)
        {
            if (targetNode is DialogueNode)
                targetNode.SetPosition(new Rect(
                    _containerCache.DialogueNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is AttributeNode)
                targetNode.SetPosition(new Rect(
                    _containerCache.AttributeNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is TraitNode)
                targetNode.SetPosition(new Rect(
                    _containerCache.TraitNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));

            if (targetNode is StoryNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.StoryNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is EndNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.EndNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is ItemNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.ItemNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is MovementNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.MovementNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is FocusNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.FocusNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
            
            if (targetNode is AnimationNode)
                targetNode.SetPosition( new Rect(
                    _containerCache.AnimationNodeDatas.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.DefaultNodeSize));
        }

        private void LinkNodes(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge
            {
                output = outputSocket,
                input = inputSocket 
            };

            tempEdge?.input.Connect(tempEdge); 
            tempEdge?.output.Connect(tempEdge); 
            _targetGraphView.Add(tempEdge); 
        }

        private void CreateNodes()
        {
            foreach(var nodeData in _containerCache.DialogueNodeDatas)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, Vector2.zero, nodeData.IsContinue ,nodeData.CharacterID, nodeData.IsEndNode, nodeData.ContinueTimer);
                tempNode.GUID = nodeData.NodeGUID; 
                tempNode.IsEndNode = nodeData.IsEndNode; 
                _targetGraphView.AddElement(tempNode); 

                var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }

            foreach(var nodeData in _containerCache.AttributeNodeDatas)
            {
                var tempNode = _targetGraphView.CreateAttributeNode("Attribute Node", Vector2.zero, nodeData.AttributeID);
                tempNode.GUID = nodeData.NodeGUID; 
                tempNode.AttributeID = nodeData.AttributeID; 
                _targetGraphView.AddElement(tempNode); 

                //var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
            }
            

            foreach (var nodeData in _containerCache.TraitNodeDatas)
            {
                var tempNode = _targetGraphView.CreateTraitNode("Trait Node", Vector2.zero, nodeData.TraitID); 
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode); 

                //var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
            }

            foreach (var nodeData in _containerCache.StoryNodeDatas)
            {
                var tempNode = _targetGraphView.CreateStoryNode("Story Node", Vector2.zero, nodeData.Id, nodeData.Value); 
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode); 
            }
            
            foreach (var nodeData in _containerCache.EndNodeDatas)
            {
                var tempNode = _targetGraphView.CreateEndNode("End Node", Vector2.zero); 
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode); 
            }
            
            foreach (var nodeData in _containerCache.MovementNodeDatas)
            {
                var tempNode = _targetGraphView.CreateMovementNode("Movement Node", Vector2.zero, nodeData.MovePos);
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode);
            }
            
            foreach (var nodeData in _containerCache.FocusNodeDatas)
            {
                var tempNode = _targetGraphView.CreateFocusNode("Focus Node", Vector2.zero, nodeData.CharacterIndex);
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode);
            }
            foreach (var nodeData in _containerCache.AnimationNodeDatas)
            {
                var tempNode = _targetGraphView.CreateAnimationNode("Animation Node", Vector2.zero, nodeData.AnimationName,
                    nodeData.Active, nodeData.CharacterIndex);
                tempNode.GUID = nodeData.NodeGUID; 
                _targetGraphView.AddElement(tempNode);
            }
        }

        private void ClearGraph()
        {
           Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID; 

           foreach(var node in Nodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));  
           
                _targetGraphView.RemoveElement(node); 
           } 

           foreach(var node in AttributeNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));  
           
                _targetGraphView.RemoveElement(node); 
           } 

           foreach(var node in TraitNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                .ForEach (edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(node);                     
           }

           foreach (var node in StoryNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }

           foreach (var node in ItemNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }
           
           foreach (var node in EndNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }
           
           foreach (var node in MovementNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }
           
           foreach (var node in FocusNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }
           
           foreach (var node in AnimationNodes)
           {
               if (node.EntryPoint) continue;
               Edges.Where(x => x.input.node == node).ToList()
                   .ForEach (edge => _targetGraphView.RemoveElement(edge));

               _targetGraphView.RemoveElement(node);          
           }
        }

        private NodeProperties FindTargetNode(string targetNodeGUID)
        {
            NodeProperties node = new NodeProperties(); 

            node = Nodes.FirstOrDefault(x => x.GUID == targetNodeGUID); 

            if (node == default)
                node = AttributeNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);
            
            if (node == default)
                node = TraitNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

            if (node == default)
                node = StoryNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

            if (node == default)
                node = EndNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

            if (node == default)
                node = ItemNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

            if (node == default)
                node = MovementNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);
            
            if (node == default)
                node = FocusNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);
            
            if (node == default)
                node = AnimationNodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

            return node; 
        }

        public List<DialogueNode> GetDialogueNodes()
        {
            List<DialogueNode> tempList = new List<DialogueNode>();

            foreach(var node in _targetGraphView.nodes.ToList())
            {
                if (node is DialogueNode)
                    tempList.Add(node as DialogueNode); 
            }

            return tempList; 
        }

        public List<AttributeNode> GetAttributeNodes()
        {
            List<AttributeNode> tempList = new List<AttributeNode>();

            foreach(var node in _targetGraphView.nodes.ToList())
            {
                if (node is AttributeNode)
                    tempList.Add(node as AttributeNode); 
            }

            return tempList; 
        }

        public List<TraitNode> GetTraitNodes()
        {
            List<TraitNode> tempList = new List<TraitNode>();

            foreach(var node in _targetGraphView.nodes.ToList())
                if (node is TraitNode)
                    tempList.Add(node as TraitNode); 
            
            return tempList; 
        }

        public List<EndNode> GetEndNodes()
        {
            List<EndNode> tempList = new List<EndNode>();

            foreach (var node in _targetGraphView.nodes.ToList())
                if (node is EndNode)
                    tempList.Add(node as EndNode); 

            return tempList;              
        }

        public List<StoryNode> GetStoryNodes()
        {
            List<StoryNode> tempList = new List<StoryNode>();

            foreach (var node in _targetGraphView.nodes.ToList())
            {
                if (node is StoryNode)
                    tempList.Add(node as StoryNode);
            }

            return tempList;
        }

        public List<ItemNode> GetItemNodes()
        {
            List<ItemNode> tempList = new List<ItemNode>();
            foreach (var node in _targetGraphView.nodes.ToList())
            {
                if (node is ItemNode)
                    tempList.Add(node as ItemNode);
            }

            return tempList;
        }
        
        public List<MovementNode> GetMovementNodes()
        {
            List<MovementNode> tempList = new List<MovementNode>();
            foreach (var node in _targetGraphView.nodes.ToList())
            {
                if (node is MovementNode)
                    tempList.Add(node as MovementNode);
            }

            return tempList;
        }
        public List<FocusNode> GetFocusNodes()
        {
            List<FocusNode> tempList = new List<FocusNode>();
            foreach (var node in _targetGraphView.nodes.ToList())
            {
                if (node is FocusNode)
                    tempList.Add(node as FocusNode);
            }

            return tempList;
        }
        
        public List<AnimationNode> GetAnimationNodes()
        {
            List<AnimationNode> tempList = new List<AnimationNode>();
            foreach (var node in _targetGraphView.nodes.ToList())
            {
                if (node is AnimationNode)
                    tempList.Add(node as AnimationNode);
            }

            return tempList;
        }
    #endif

    }
}
