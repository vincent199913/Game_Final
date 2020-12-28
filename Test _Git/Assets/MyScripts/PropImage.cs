using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PropImage : MonoBehaviour
{
    public bool clicked = false;
    public int prop_cat = 0;
    
    public void onClick(){
        clicked = !clicked;
    }
}
