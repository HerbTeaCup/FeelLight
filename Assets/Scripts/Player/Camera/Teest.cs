using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teest : MonoBehaviour
{
    [SerializeField] CameraAirRotator airRotator;

    private void Update()
    {
        this.transform.LookAt(airRotator.targetDir);
    }
}
