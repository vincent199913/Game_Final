using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDrawEraser : Prop
{
    public GameObject eraserPrefab;
    public int limitNumber = 1;

    GameObject newObject;
    GameObject EraserPrefab;

    Camera cam;
    public bool canDraw;
    Eraser er;

    void Start()
    {
        cam = Camera.main;
        er = new Eraser();
        EraserPrefab = new GameObject();
        //limitNumber = 0;        
    }

    void FixedUpdate()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (canDraw)
        {
            if (Input.GetMouseButtonDown(1) && limitNumber > 0 && newObject == null)
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("BeginErase", RpcTarget.AllBuffered, mousePosition);
            }


            if (newObject != null)
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("Erasing", RpcTarget.AllBuffered, mousePosition, limitNumber);
            }


            if (Input.GetMouseButtonUp(1))
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
    void Erasing(Vector2 mousePosition,int limit)
    {
        // Eraser er = newObject.transform.GetChild(0).gameObject.GetComponent<Eraser>();        
        if(EraserPrefab != null)
            EraserPrefab.TryGetComponent<Eraser>(out er);        
        if (er)
        {
            limit -= er.destroyNum;
            //limitNumber -= er.destroyNum;
        }            
        limitNumber = limit;
        if (limit > 0)
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