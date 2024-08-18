using UnityEngine;
using Anchry.Dialogue;

public class StoryNode : NodeProperties
{
    public string StoryString;
    public int StoryPointID; 
    
    public void TranslateAnswerData(string value)
    {
        string[] storyPont = System.Enum.GetNames(typeof(DialougeEnum.StoryPoints));

        for(int i = 0; i < storyPont.Length; i++)
        {
            if (value == storyPont[i])
            {
                StoryPointID = i; 
                return;
            }
        }
        Debug.LogError($"[AnswerNode] Error setting id for {value}"); 
    }
}
