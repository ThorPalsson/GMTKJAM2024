using UnityEngine;
# if UNITY_EDITOR 
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
#endif

namespace Anchry.Dialogue
{
# if UNITY_EDITOR 

    [CreateAssetMenu(fileName = "NodeSearchWindow", menuName = "DialogueSystem/NodeSearchWindow", order = 0)]
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView _graphView; 
        private EditorWindow _window; 
        private Texture2D _indentationIcon;
        private bool _generatingContinue; 

        public void Init(EditorWindow window, DialogueGraphView graphView)
        {
            _graphView = graphView; 
            _window = window; 

            _indentationIcon = new Texture2D (1,1); 
            _indentationIcon.SetPixel(0,0, new Color(0,0,0,0));
            _indentationIcon.Apply(); 
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"),0),
                //new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),1), 
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    userData = new DialogueNode(), level = 1
                },
                new SearchTreeEntry(new GUIContent("Trait Node", _indentationIcon))
                {
                    userData = new TraitNode(), level = 1
                },
                new SearchTreeEntry(new GUIContent("Item Node", _indentationIcon))
                {
                    userData = new ItemNode(), level = 1
                },
                new SearchTreeEntry(new GUIContent("Story Node", _indentationIcon))
                {
                    userData = new StoryNode(), level = 1
                },
                new SearchTreeEntry(new GUIContent("End Node", _indentationIcon))
                {
                    userData = new EndNode(), level =  1
                },
                new SearchTreeEntry(new GUIContent("Movement Node", _indentationIcon))
                {
                    userData = new MovementNode(), level =  1
                },
                new SearchTreeEntry(new GUIContent("Focus Node", _indentationIcon))
                {
                    userData = new FocusNode(), level =  1
                },
                new SearchTreeEntry(new GUIContent("Animation Node", _indentationIcon))
                {
                    userData = new AnimationNode(), level =  1
                },
                new SearchTreeEntry(new GUIContent("ContinueNode", _indentationIcon))
                {
                    userData = new DialogueNode(), level = 1
                }
                
            }; 
            return tree; 
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, 
                context.screenMousePosition - _window.position.position); 
            var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition); 

            switch(SearchTreeEntry.userData)
            {
                case DialogueNode dialogueNode: 
                    _graphView.GenerateDialogueNode("Dialogue Node", localMousePosition, SearchTreeEntry.content.text == "ContinueNode"); 
                    return true;  
                case AttributeNode attributeNode:
                    _graphView.GenerateAttributeNode("Attribute Node", localMousePosition); 
                    return true; 
                case TraitNode traitNode:
                    _graphView.GenerateTraitNode("Trait Node", localMousePosition); 
                    return true;
                case StoryNode storyNode:
                    _graphView.GenerateStoryNode("Story Node", localMousePosition);
                    return true;
                case EndNode endNode:
                    _graphView.GenereateEndNode("End Node", localMousePosition);
                    return true;
                case MovementNode moveNode:
                    _graphView.GenerateMovementNode("Movement Node", localMousePosition);
                    return true;
                case FocusNode focusNode:
                    _graphView.GenerateFocusNode("Focus Node", localMousePosition);
                    return true;
                case AnimationNode animationNode:
                    _graphView.GenerateAnimationNode("Animation Node", localMousePosition);
                    return true;
                default:
                    return false;
            }
        }
    }
#endif

}

