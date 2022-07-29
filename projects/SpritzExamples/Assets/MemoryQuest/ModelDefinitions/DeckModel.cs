using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck.asset", menuName = "Spritz/Create new Deck Model")]
public class DeckModel : ScriptableObject
{
    public CardModel[] counterAttack;
    public CardModel[] cards;
}
