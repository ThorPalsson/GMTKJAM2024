using System;
using UnityEngine;

namespace Anchry.Dialogue
{
    public class DialougeEnum
    {
        public enum Attribute
        {
            Strength,
            Dexterety,
            Size,
            Constitution,
            Appearance, 
            MentalPower,
            Intelligence
        }; 

        public enum PlayerTrait
        {
            Herculean,
            Metaphysical, 
            Alacritous, 
            Perspicacious, 
            Circumspect, 
            Dipsomaniacal
        }

        public enum Crew
        {
            SavannahFitzgerald,
            PhilipPresho,
            MercuryEola,
            EugeneToeberg,
            ArlingtonSugarland,
            Lubbock,
        }

        public enum StoryPoints
        {
            PlayerName = 0, 
            Victim = 1, 
            PlaceOfBody = 2, 
            MurderWeapon = 3,
        }


        public Attribute AllAttributes; 
        public PlayerTrait Traits; 
        public Crew CrewMembers;
        public StoryPoints StoryPoint;

    }
}
