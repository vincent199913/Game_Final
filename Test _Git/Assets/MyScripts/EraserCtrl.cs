using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserCtrl : MonoBehaviour
{
    public ParticleSystem system;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        system.Play();
    }
}
