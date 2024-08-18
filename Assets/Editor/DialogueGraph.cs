using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;
#endif

namespace Anchry.Dialogue
{
    [ExecuteInEditMode]
    public class DialogueGraph : EditorWindow 
    {
        # if UNITY_EDITOR 

        private DialogueGraphView _graphView; 
        private string _fileName = "New Narrative";


        [MenuItem("DialogueSystem/Dialogue Graph")]
        private static void ShowWindow() 
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("DialogueGraph");
        }

        private void OnEnable() 
        {
            ConstructGraphView();
            GenerateToolBar();
            GenerateMiniMap();
            GenerateBlackBoard();
        }


        private void OnDisable() 
        {
            if (_graphView != null)
                rootVisualElement.Remove(_graphView); 
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph", 
            }; 
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView); 
        }

        private void GenerateToolBar()
        {
            var toolbar = new Toolbar(); 

            var fileNameTextField = new TextField("FileName"); 
            fileNameTextField.SetValueWithoutNotify(_fileName); 
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);  

            toolbar.Add(new Button(()=> RequestDataOperation(true)){text = "Save Data"}); 
            toolbar.Add(new Button(()=> RequestDataOperation(false)){text = "Load Data"}); 
            rootVisualElement.Add(toolbar);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap{anchored = true};
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(10, 30)); 
            miniMap.SetPosition(new Rect(cords.x, cords.y,200,140));
            _graphView.Add(miniMap); 
        }
        private void GenerateBlackBoard()
        {
            ExposedPropertyContainer propertyContainer = Resources.Load<ExposedPropertyContainer>("Containers/PropertyContainer");

            if (propertyContainer == null)
            {
                propertyContainer = ScriptableObject.CreateInstance<ExposedPropertyContainer>();
                AssetDatabase.CreateAsset(propertyContainer, $"Assets/Resources/Containers/PropertyContainer.asset"); 
            }
            
            var blackBoard = new Blackboard(_graphView); 
            blackBoard.Add(new BlackboardSection{title = "Properties"}); 
            blackBoard.addItemRequested = _blackboard => {_graphView.AddPropertyToBlackBoard(new ExposedProperty());};
            blackBoard.editTextRequested = (blackBoard1, element, newValue) => 
            {
                var oldPropertyName = ((BlackboardField)element).text; 
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, chose another one or die!", "I want to live!"); 
                    return; 
                }

                var propertyIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName); 
                _graphView.ExposedProperties[propertyIndex].PropertyName = newValue; 

                ((BlackboardField)element).text = newValue;  
               
            };

            blackBoard.SetPosition(new Rect(10,180,300,300)); 
            _graphView.Add(blackBoard); 
            _graphView.Blackboard = blackBoard;

            foreach (var property in propertyContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(property);
            }

        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name Jonathan!", "Please enter a valid file name you dumbo", "I am stupid"); 
                return; 
            }

            var saveUtility = GraphSaveUtility.GetInstance(_graphView); 

            if (save)
            {
                saveUtility.SaveGraph(_fileName); 
            }
            else 
            {
                saveUtility.LoadGraph(_fileName); 
            }
        }
    #endif
    }

}

