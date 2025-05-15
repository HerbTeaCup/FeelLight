using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator _anim;
    
    
    
    void Start()
    {
        _anim = GetComponent<Animator>();
    }
}
