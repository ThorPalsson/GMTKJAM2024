# if !UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
#endif


namespace Anchry.Dialogue
{
# if !UNITY_EDITOR 

    public class ActorWindow : EditorWindow 
    {
        string ActorName = "New Actor"; 
        private bool showActors; 
        private bool editActor; 

        private Actor dummyActor; 

        [MenuItem("DialogueSystem/ActorWindow")]
        private static void ShowWindow() 
        {
            var window = GetWindow<ActorWindow>();
            window.titleContent = new GUIContent("ActorWindow");
            window.Show();
        }

        private void OnGUI() 
        {
            ActorContainer actorContainer = (ActorContainer)AssetDatabase.LoadAssetAtPath("Assets/Resources/ActorContainer.asset", typeof(ActorContainer)); 

            if (!showActors && !editActor)
            {
                GUILayout.Label("Create an Actor", EditorStyles.boldLabel); 
                ActorName = GUILayout.TextField(ActorName); 

                if (actorContainer == null)
                {
                    EditorUtility.DisplayDialog("No actor container found", "Please create an actor container and name it ActorContainer", "Yes daddy!");
                    this.Close();
                }

                if (GUILayout.Button("Create Actor"))
                {
                    if (ActorName == "New Actor" || string.IsNullOrEmpty(ActorName) || ActorName == "Jonathan")
                    {
                        EditorUtility.DisplayDialog("Invalid Actor Name", "Please enter a real name looser", 
                            "I have dissapointed my ancestors"); 
                        return; 
                    }

                    if (actorContainer.Actors.Any(c => c.ActorName == ActorName))
                    {
                        EditorUtility.DisplayDialog("Actor Name In Use", "There is already an actor with that name",
                            "ROHIRRIM!!!"); 
                        return; 
                    }

                    Actor actor = new Actor
                    {
                        ActorName = ActorName
                    }; 
                    actorContainer.Add(actor); 

                    ActorName = "New Actor"; 
                }

                if (GUILayout.Button("Load Actors"))
                    showActors = true; 
            }

            if (showActors)
            {
                GUILayout.Label("Select Actor to Edit", EditorStyles.boldLabel); 

                if (GUILayout.Button("Cancel editing"))
                {
                    showActors = false; 
                }

                GUILayout.Space(10); 

                for(int i = 0; i < actorContainer.Actors.Count; i++)
                    if (GUILayout.Button(actorContainer.Actors[i].ActorName))
                        ShowcaseActor(actorContainer.Actors[i]); 

            }

            if (editActor)
            {
                GUILayout.Label("Edit Actor", EditorStyles.boldLabel); 
                dummyActor.ActorName = GUILayout.TextField(dummyActor.ActorName); 

                if (GUILayout.Button("Finilize Editing"))
                {
                    actorContainer.Actors.Where(c => c.ActorID == dummyActor.ActorID).Select(c => {c = dummyActor; return c;}); 
                    dummyActor = null; 
                    editActor = false; 
                }
            }
        }

        private void ShowcaseActor(Actor actorData)
        {
            showActors = false; 
            editActor = true; 
            dummyActor = actorData; 
        }
    }
#endif
}
