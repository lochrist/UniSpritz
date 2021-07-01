using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class EmojiComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Spritz.Initialize(this.gameObject, new EmojiGame());
    }
}
