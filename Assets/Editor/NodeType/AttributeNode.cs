using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttributeNode : NodeProperties
{
    public int AttributeID;
    public string AttributeName; 


    public void TranslateAttributeString(string attributeName)
    {
        int id = -1; 
        switch(attributeName)
        {
            case "Strength":
                id = 0; 
                break; 
            case "Dexterety":
                id = 1; 
                break; 
            case "Size":
                id = 2;
                break; 
            case "Constitution":
                id = 3; 
                break; 
            case "Appearance": 
                id = 4; 
                break; 
            case "MentalPower":
                id = 5;
                break; 
            case "Intelligence":
                id = 6;
                break; 
        }

        if (id != -1)
        {
            AttributeID = id; 
            AttributeName = attributeName; 
        }
        else 
            Debug.LogError($"[AttributeNode] Attribute {attributeName} doesn't exist"); 
    }

}
