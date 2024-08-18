using UnityEngine;
using Anchry.Dialogue; 
using System; 

[Serializable]
public class TraitNode : NodeProperties
{
    public int TraitID; 
    public string TraitName; 

    public void TranslateTraitString(string trait)
    {
        string[] traitNames = System.Enum.GetNames(typeof(DialougeEnum.PlayerTrait));

        for(int i = 0; i < traitNames.Length; i++)
        {
            if (trait == traitNames[i])
            {
                TraitID = i; 
                TraitName = trait; 
                return; 
            }
        }

        Debug.LogError($"[TraitNode] Error setting id for {trait}"); 
    }
}
