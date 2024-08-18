using System.Net.Mime;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR 
using UnityEditor.Experimental.GraphView; 
using UnityEngine.UIElements;
using UnityEditor;
#endif
using UnityEditor.UIElements;
using UnityEngine.Rendering;

namespace Anchry.Dialogue
{
# if UNITY_EDITOR 

    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
        public Blackboard Blackboard;
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow; 
        // private bool showFoldOut = false;
        private DialougeEnum _graphEnums = new DialougeEnum();

        private CharacterContainer _container;
        private List<String> _characters;

        private List<String> _items; 

        public DialogueGraphView(EditorWindow window)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/DialogueGraph")); 
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger()); 
            this.AddManipulator(new SelectionDragger()); 
            this.AddManipulator(new RectangleSelector()); 

            var grid = new GridBackground();
            Insert(0,grid); 
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode()); 
            AddSearchWindow(window);
            
            _container = Resources.Load<CharacterContainer>("Containers/CharacterContainer");
            _characters = _container.Characters.Select(x => x.CharacterName).ToList();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>(); 

            ports.ForEach(port => 
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port); 
            });
            return compatiblePorts; 
        }

        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); 
        }

        private DialogueNode GenerateEntryPointNode()
        {
            var node = new DialogueNode
            {
                title = "START", 
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true
            }; 

            var generatedPort = GeneratePort(node, Direction.Output); 
            generatedPort.portName = "First Node"; 
            node.outputContainer.Add(generatedPort); 

            node.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/EntryNode"));

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable; 

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(400,200,100,150)); 
            return node; 
        }

        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
        {
            var localPropertyName = exposedProperty.PropertyName; 
            var localPropertyValue = exposedProperty.PropertyValue;

            while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                localPropertyName = $"{localPropertyName}(1)"; 

            var property = new ExposedProperty();
            property.PropertyName = localPropertyName; 
            property.PropertyValue = localPropertyValue; 

            ExposedProperties.Add(property); 

            var container = new VisualElement();
            var blackboardField = new BlackboardField{text = property.PropertyName, typeText = "string property"};
            container.Add(blackboardField); 

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            }; 
            propertyValueTextField.RegisterValueChangedCallback(evt => 
            {
                var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName); 
                ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue; 
            });
            var blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField); 
            container.Add(blackBoardValueRow); 

            Blackboard.Add(container); 
        }

        public void GenerateAttributeNode(string nodeName, Vector2 mousePosition)
        {
            AddElement(CreateAttributeNode(nodeName, mousePosition)); 
        }

        public void GenerateTraitNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateTraitNode(nodeName, mousePosition));

        public TraitNode CreateTraitNode(string nodeName, Vector2 mousePosition, int traitID = -1)
        {
            var traitNode = new TraitNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString()
            }; 

            traitNode.inputContainer.Add(CreateInputPort(traitNode)); 

            //TODO: Create special style sheet for new node
            traitNode.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/Node")); 

            //Generate success port
            var equippedPort = GeneratePort(traitNode, Direction.Output); 
            var equippedPortOldLabel = equippedPort.contentContainer.Q<Label>("type"); 
            equippedPort.contentContainer.Remove(equippedPortOldLabel); 
            string equippedPortName = "Equipped"; 

            var successTextField = new TextField
            {
                name = string.Empty,
                value = equippedPortName,
                focusable = false
            }; 

            equippedPort.contentContainer.Add(new Label("  ")); 
            equippedPort.contentContainer.Add(successTextField); 

            //Generate fail port
            var negativePort = GeneratePort(traitNode, Direction.Output); 
            var negativePortOldLabel = negativePort.contentContainer.Q<Label>("type"); 
            negativePort.contentContainer.Remove(negativePortOldLabel); 
            string negativePortName = "Unequipped"; 
        
            var failTextField = new TextField
            {
                name = string.Empty,
                value = negativePortName,
                focusable = false
            }; 
            negativePort.contentContainer.Add(new Label("  ")); 
            negativePort.contentContainer.Add(failTextField); 

            var dropdown = new EnumField("Trait", _graphEnums.Traits);
            dropdown.RegisterValueChangedCallback(evt => 
            {
                traitNode.TranslateTraitString(evt.newValue.ToString());
            });
            if (traitID != -1)
            {
                _graphEnums.Traits = (DialougeEnum.PlayerTrait)traitID; 
                traitNode.TranslateTraitString(_graphEnums.Traits.ToString()); 
                dropdown.Init(_graphEnums.Traits); 
            }
            traitNode.mainContainer.Add(dropdown); 

            equippedPort.portName = equippedPortName; 
            negativePort.portName = negativePortName; 
            traitNode.outputContainer.Add(equippedPort); 
            traitNode.outputContainer.Add(negativePort);
            
            RefreshNode(traitNode, mousePosition);

            return traitNode; 
        }

        public AttributeNode CreateAttributeNode(string nodeName, Vector2 mousePosition, int attributeID = -1)
        {
            var attributeNode = new AttributeNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString()
            }; 

            attributeNode.inputContainer.Add(CreateInputPort(attributeNode)); 

            //TODO: Create special style sheet for new node
            attributeNode.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/Node")); 

            //Generate success port
            var successPort = GeneratePort(attributeNode, Direction.Output); 
            var successPortOldLabel = successPort.contentContainer.Q<Label>("type"); 
            successPort.contentContainer.Remove(successPortOldLabel); 
            string successPortName = "Success"; 

            var successTextField = new TextField
            {
                name = string.Empty,
                value = successPortName,
                focusable = false
            }; 

            successPort.contentContainer.Add(new Label("  ")); 
            successPort.contentContainer.Add(successTextField); 

            //Generate fail port
            var failPort = GeneratePort(attributeNode, Direction.Output); 
            var failPortOldLabel = failPort.contentContainer.Q<Label>("type"); 
            failPort.contentContainer.Remove(failPortOldLabel); 
            string failPortName = "Fail"; 
        
            var failTextField = new TextField
            {
                name = string.Empty,
                value = failPortName,
                focusable = false
            }; 
            failPort.contentContainer.Add(new Label("  ")); 
            failPort.contentContainer.Add(failTextField); 

            var dropdown = new EnumField("Attribute", _graphEnums.AllAttributes);
            dropdown.RegisterValueChangedCallback(evt => 
            {
                attributeNode.TranslateAttributeString(evt.newValue.ToString());
            });
            if (attributeID != -1)
            {
                _graphEnums.AllAttributes = (DialougeEnum.Attribute)attributeID; 
                dropdown.Init(_graphEnums.AllAttributes); 
            }
            attributeNode.mainContainer.Add(dropdown); 

            successPort.portName = successPortName; 
            failPort.portName = failPortName; 
            attributeNode.outputContainer.Add(successPort); 
            attributeNode.outputContainer.Add(failPort); 

            RefreshNode(attributeNode, mousePosition);
            
            
            return attributeNode; 
        }

        public void GenerateDialogueNode(string nodeName, Vector2 mousePosition, bool isContinue)
        {
            AddElement(CreateDialogueNode(nodeName, mousePosition, isContinue)); 
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 position, bool isContinue, int characterId = -1 ,bool isEndNode = false, string continueTimer = "")
        {
            var dialogueNode = new DialogueNode
            {
                title = nodeName, 
                DialogueText = nodeName, 
                GUID = Guid.NewGuid().ToString(),
                IsContinue = isContinue,
                ContinueTimer = continueTimer
            };

            dialogueNode.inputContainer.Add(CreateInputPort(dialogueNode)); 
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/Node"));

            if (!isContinue)
            {
                var button = new Button(() => {AddChoicePort(dialogueNode);}); 
                button.text = "New Choice"; 
                dialogueNode.titleContainer.Add(button);
            }
            else
            {
                var timerField = new TextField("Timer");
                timerField.RegisterValueChangedCallback(evt => 
                {
                    dialogueNode.ContinueTimer = evt.newValue;
                }); 
                timerField.SetValueWithoutNotify(dialogueNode.ContinueTimer); 
                dialogueNode.outputContainer.Add(timerField);
                
                dialogueNode.outputContainer.Add(CreateOutputPort(dialogueNode, "Continue"));
            }

            var dropDownIndex = characterId == -1 ? 0 : characterId;
            var dropDown = new DropdownField("Character", _characters, dropDownIndex);
            dropDown.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.CharacterIndex = _container.GetIndex(evt.newValue);
            });

            dialogueNode.mainContainer.Add(dropDown);

            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt => 
            {
                dialogueNode.DialogueText = evt.newValue;
                dialogueNode.title = evt.newValue;
            }); 
            textField.SetValueWithoutNotify(dialogueNode.title); 
            dialogueNode.mainContainer.Add(textField);

            if (!isContinue)
            {
                var boolField = new Toggle("Is End Node");
                boolField.RegisterValueChangedCallback(evt => 
                {
                    dialogueNode.IsEndNode = evt.newValue; 
                }); 
                dialogueNode.IsEndNode = isEndNode; 
                boolField.value = isEndNode; 
                dialogueNode.topContainer.Add(boolField);
            }

            RefreshNode(dialogueNode, position);

            return dialogueNode; 
        }
               public void GenerateStoryNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateStoryNode(nodeName, mousePosition));

        public StoryNode CreateStoryNode(string nodeName, Vector2 mousePosition, int id = -1, string value = "")
        {
            var storyNode = new StoryNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
            };
            
            storyNode.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/StoryNode"));
            storyNode.inputContainer.Add(CreateInputPort(storyNode));
            
            var dropdown = new EnumField("Answer Type", _graphEnums.StoryPoint);
            dropdown.RegisterValueChangedCallback(evt => 
            {
                storyNode.TranslateAnswerData(evt.newValue.ToString());
            });
            if (id != -1)
            {
                _graphEnums.StoryPoint = (DialougeEnum.StoryPoints)id; 
                storyNode.TranslateAnswerData(_graphEnums.StoryPoint.ToString()); 
                dropdown.Init(_graphEnums.StoryPoint); 
            }
            storyNode.extensionContainer.Add(dropdown); 
            
            var storyString = new TextField(string.Empty);
            storyString.RegisterValueChangedCallback(evt => 
            {
                storyNode.StoryString = evt.newValue;
                storyNode.title = evt.newValue;
            }); 
            storyString.SetValueWithoutNotify(storyNode.title); 
            storyNode.mainContainer.Add(storyString); 
            
            storyNode.outputContainer.Add(CreateOutputPort(storyNode));
            RefreshNode(storyNode, mousePosition);
            
            return storyNode;
        }

        public void GenereateEndNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateEndNode(nodeName, mousePosition));
        public EndNode CreateEndNode(string nodeName, Vector2 mousePosition, int id = -1, int value = -1)
        {
            var endNode = new EndNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            endNode.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/EndNode"));
            endNode.inputContainer.Add(CreateInputPort(endNode)); 
            RefreshNode(endNode, mousePosition);
            return endNode; 
        }
        
        public void GenerateMovementNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateMovementNode(nodeName, mousePosition, Vector3.zero));

        public MovementNode CreateMovementNode(string nodeName, Vector2 mousePosition, Vector3 pos)
        {
            var node = new MovementNode()
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
            };
            
            node.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/StoryNode"));
            node.inputContainer.Add(CreateInputPort(node));

            var vectorField = new Vector3Field("Movement Position");
            vectorField.RegisterValueChangedCallback(evt =>
            {
                node.Position = evt.newValue;
            });
            vectorField.SetValueWithoutNotify(pos);
            node.mainContainer.Add(vectorField);
            
            node.outputContainer.Add(CreateOutputPort(node));
            RefreshNode(node, mousePosition);
            
            return node;
        }
        
        public void GenerateFocusNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateFocusNode(nodeName, mousePosition));

        public FocusNode CreateFocusNode(string nodeName, Vector2 mousePosition, int id = -1)
        {
            var node = new FocusNode()
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
            };
            
            node.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/StoryNode"));
            node.inputContainer.Add(CreateInputPort(node));

            var dropDownIndex = id == -1 ? 0 : id;
            var dropDown = new DropdownField("Character", _characters, dropDownIndex);
            dropDown.RegisterValueChangedCallback(evt =>
            {
                node.CharacterIndex = _container.GetIndex(evt.newValue);
            });
            node.mainContainer.Add(dropDown);

            node.outputContainer.Add(CreateOutputPort(node));
            RefreshNode(node, mousePosition);
            
            return node;
        }
        
        public void GenerateAnimationNode(string nodeName, Vector2 mousePosition) 
            => AddElement(CreateAnimationNode(nodeName, mousePosition));

        public AnimationNode CreateAnimationNode(string nodeName, Vector2 mousePosition, string animName = "", bool isActive = false, int id = -1)
        {
            var node = new AnimationNode()
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
            };
            
            node.styleSheets.Add(Resources.Load<StyleSheet>("StyleSheets/StoryNode"));
            node.inputContainer.Add(CreateInputPort(node));

            var animationNameField = new TextField("Animation Name");
            animationNameField.RegisterValueChangedCallback(evt => 
            {
                node.AnimationName = evt.newValue;
                node.title = evt.newValue;
            }); 
            animationNameField.SetValueWithoutNotify(animName); 
            node.mainContainer.Add(animationNameField); 
            
            var boolField = new Toggle("Active");
            boolField.RegisterValueChangedCallback(evt => 
            {
                node.Active = evt.newValue; 
            }); 
            node.Active = isActive; 
            boolField.value = isActive; 
            node.mainContainer.Add(boolField);
            
            var dropDownIndex = id == -1 ? 0 : id;
            var dropDown = new DropdownField("Character", _characters, dropDownIndex);
            dropDown.RegisterValueChangedCallback(evt =>
            {
                node.CharacterIndex = _container.GetIndex(evt.newValue);
            });
            node.contentContainer.Add(dropDown);

            node.outputContainer.Add(CreateOutputPort(node));
            RefreshNode(node, mousePosition);
            
            return node;
        }

        public void AddChoicePort(NodeProperties dialogueNode, string overriddenPortName = "")
        {
            var generatedPort = GeneratePort(dialogueNode, Direction.Output); 
            var oldLabel = generatedPort.contentContainer.Q<Label>("type"); 
            generatedPort.contentContainer.Remove(oldLabel);
            
            var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count; 

            var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? 
                $"Choice {outputPortCount + 1}" : overriddenPortName; 

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName
            }; 
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue); 
            generatedPort.contentContainer.Add(new Label("  ")); 
            generatedPort.contentContainer.Add(textField); 
            var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
            {
                text = "X"
            }; 

            generatedPort.contentContainer.Add(deleteButton); 

            generatedPort.portName = choicePortName; 
            dialogueNode.outputContainer.Add(generatedPort); 
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        private Port CreateInputPort(Node node)
        {
            Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";

            return inputPort;
        }

        private Port CreateOutputPort(Node node, string portName = "output")
        {
            Port outputPort = GeneratePort(node, Direction.Output);

            outputPort.contentContainer.Add(new Label("   "));
            outputPort.portName = portName;

            return outputPort;
        }

        private void RefreshNode(Node node, Vector2 mousePosition)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(mousePosition, DefaultNodeSize));
        }

        private void RemovePort(NodeProperties dialogueNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => 
                x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
             
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge); 
                RemoveElement(targetEdge.First()); 
            }

            dialogueNode.outputContainer.Remove(generatedPort); 
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }
        private void AddSearchWindow(EditorWindow window)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(window, this); 
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow); 
        }
    }
#endif

}
