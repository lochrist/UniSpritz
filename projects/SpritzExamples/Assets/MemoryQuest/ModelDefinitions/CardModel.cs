using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard.asset", menuName = "Spritz/Create new Card Model")]
public class CardModel : ScriptableObject
{
    public Sprite[] cardFrames;
    public int dmg;
}
