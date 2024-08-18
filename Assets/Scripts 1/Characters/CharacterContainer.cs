using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New CharacterContainer", menuName = "Characters/Generate CharacterContainer")]
public class CharacterContainer : ScriptableObject
{
    public List<Character> Characters;


    public int GetIndex(string name)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].CharacterName == name)
                return i; 
        }

        return -1;
    }
}
