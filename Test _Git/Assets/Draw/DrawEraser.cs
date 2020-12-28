using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DrawEraser : Prop
{
    public GameObject eraserPrefab;
    public int limitNumber = 1;

    GameObject newObject;
    GameObject EraserPrefab;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        //limitNumber = 0;        
    }

    void FixedUpdate()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (limitNumber > 0) {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("BeginErase", RpcTarget.AllBuffered, mousePosition);                
            }
                

            if (newObject != null)
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("Erasing", RpcTarget.AllBuffered, mousePosition);
            }
                

            if (Input.GetMouseButtonUp(0))
                this.photonView.RPC("EndErase", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    void BeginErase(Vector2 mousePosition)
    {        
        newObject = new GameObject("EraserObject");
        newObject.transform.position = mousePosition;
        EraserPrefab = Instantiate(eraserPrefab, newObject.transform);        
    }
    [PunRPC]
    void Erasing(Vector2 mousePosition)
    {
        // Eraser er = newObject.transform.GetChild(0).gameObject.GetComponent<Eraser>();
        Eraser er = EraserPrefab.GetComponent<Eraser>();
        if(er)
            limitNumber -= er.destroyNum;
        if (limitNumber > 0)
        {            
            newObject.transform.position = mousePosition;
        }
        else
        {
            EndErase();
        }
    }
    [PunRPC]
    void EndErase()
    {
        // update state
        used_up = true;
        Destroy(newObject);
        newObject = null;
    }
}