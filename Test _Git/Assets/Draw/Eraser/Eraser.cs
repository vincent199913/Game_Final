using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    [HideInInspector] public int destroyNum = 0;
    List<Transform> mplayer;
    private void Start()
    {               
        mplayer = new List<Transform>();
        Destroy(this.gameObject, 1f);
    }
    private void Update()
    {        
            
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        // may need to judge tag
        if (col.gameObject.layer == 12 && col.gameObject.tag != "Player")
        {
            Debug.Log("NAME: " + col.gameObject.name);
            /*if(col.transform.parent)
                FindChildTransform(col.transform.parent.gameObject);
            
            foreach(var _mplayer in mplayer)
            {
                Debug.Log("mplayer: "+_mplayer.gameObject.name);
                _mplayer.SetParent(null);
            }*/
            if (col.gameObject.name == "RightDoor(Clone)" || col.gameObject.name == "Bow(Clone)")
            {
                if (col.gameObject.transform.parent.transform.parent.gameObject != null)
                {
                    Destroy(col.gameObject.transform.parent.transform.parent.gameObject);
                    Debug.Log(col.gameObject.name);
                    destroyNum++;
                    return;
                }
            }
            else if (col != null)
            {
                if (col.gameObject != null)
                {
                    if (col.gameObject.transform.parent.gameObject != null)
                    {
                        Destroy(col.gameObject.transform.parent.gameObject);
                        Debug.Log(col.gameObject.name);
                        destroyNum++;
                    }
                }                
            }
        }
    }/*
    public void FindChildTransform(GameObject parent)
    {
        Transform child = null;

        foreach (Transform trans in parent.transform)
        {
            if (trans.gameObject.tag == "Player")
            {
                child = trans;                
                
                if (child != null)
                    mplayer.Add(trans);
            }
            else
            {
                if(trans != null)
                    FindChildTransform(trans.gameObject);                
            }
        }

        //return child;
    }*/
}

