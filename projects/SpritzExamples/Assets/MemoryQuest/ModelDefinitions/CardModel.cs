using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MQ
{
    public enum TraitType
    {
        Armor,
        Weapon,
        Ally,
        Trap,
        Spell,
        Item,
        Potion
    }

    [Serializable]
    public struct Trait
    {
        public TraitType type;
        public int modifier;
    }
}

[CreateAssetMenu(fileName = "NewCard.asset", menuName = "Spritz/Create new Card Model")]
public class CardModel : ScriptableObject
{
    public Sprite[] cardFrames;
    public MQ.Trait[] traits;
    public bool wildMatch;
}
