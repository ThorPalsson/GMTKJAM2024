using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Generate Character")]
public class Character : ScriptableObject
{
    public int CharacterId;
    public string CharacterName;
    public string UnknownName;
    public Sprite CharacterImage;
    public Color CharacterUiColor;

    public bool IsKnown;

    public int Initiative = 0; 
}
