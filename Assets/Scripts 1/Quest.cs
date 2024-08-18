using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "New Quest", order = 0)]
public class Quest : ScriptableObject
{
    public string QuestText;
    public Quest NextQuest;
    public bool QuestComplete; 
}