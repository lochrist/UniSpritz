using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class KenneyComponent : MonoBehaviour
{    
    void Awake()
    {
        Spritz.Initialize(this.gameObject, new KenneyGame());
    }
}
