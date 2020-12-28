using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnCtrl : MonoBehaviour
{
    public Image img2;
    public Image img3;
    public Image img4;

    // Start is called before the first frame update
    void Start()
    {
        img2.enabled = false;
        img3.enabled = false;
        img4.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        Debug.Log("show");
        img2.enabled = true;
        img3.enabled = true;
        img4.enabled = true;
    }

    public void Hide()
    {
        Debug.Log("Hide");
        img2.enabled = false;
        img3.enabled = false;
        img4.enabled = false;
    }
}
