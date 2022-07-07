using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter.asset", menuName = "Spritz/Create new Character Model")]
public class CharacterModel : ScriptableObject
{
    public string characterName;
    public int hp;
    public Sprite[] characterFrames;
    public DeckModel deck;
}
