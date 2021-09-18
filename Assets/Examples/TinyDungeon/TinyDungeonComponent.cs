using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TinyDungeonComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Spritz.Initialize(this.gameObject, new TinyDungeonGame());
    }
}
