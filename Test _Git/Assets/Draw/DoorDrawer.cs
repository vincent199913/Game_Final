using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorDrawer : Prop
{
    public GameObject LeftDoorPrefab;
    public GameObject RightDoorPrefab;
    public GameObject effect;
    public int limitNumber = 1;
    public float limitLength = 10.0f;

    GameObject newObject;
    GameObject rightDoorObject;
    bool isPress = false;
    Vector2 leftDoorShift;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        BoxCollider2D bc = RightDoorPrefab.GetComponent<BoxCollider2D>();
        leftDoorShift = new Vector2(-bc.size.x, 0);
    }

    void Update()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (limitNumber > 0 && newObject == null) {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            this.photonView.RPC("CreateInstance", RpcTarget.AllBuffered, mousePosition);
            limitNumber--;
        }

        if(newObject != null)
        {
            if (!isPress)
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("MoveAllDoor", RpcTarget.AllBuffered, mousePosition);
            }
            else
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("MoveRightDoor", RpcTarget.AllBuffered, mousePosition);
            }                

            if (Input.GetMouseButtonDown(0)) {
                isPress = true;
            }

            if (Input.GetMouseButtonUp(0)) {
                this.photonView.RPC("EndMove", RpcTarget.AllBuffered);
                isPress = false;
            }
        }
    }
    [PunRPC]
    void CreateInstance(Vector2 mousePosition)
    {
        // left door
        // = cam.ScreenToWorldPoint(Input.mousePosition);
        newObject = new GameObject("DoorObject");
        newObject.transform.position = mousePosition + leftDoorShift;
        Instantiate(LeftDoorPrefab, newObject.transform);

        // right door
        rightDoorObject = new GameObject("RightDoorObject");
        rightDoorObject.transform.position = mousePosition;
        Instantiate(RightDoorPrefab, rightDoorObject.transform);
        rightDoorObject.transform.SetParent(newObject.transform);
    }
    [PunRPC]
    void MoveAllDoor(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        newObject.transform.position = mousePosition + leftDoorShift;
    }
    [PunRPC]
    void MoveRightDoor(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        rightDoorObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        if(Vector3.Distance(rightDoorObject.transform.position, newObject.transform.position) > limitLength) {
            Vector3 vec = (rightDoorObject.transform.position - newObject.transform.position).normalized * limitLength;
            rightDoorObject.transform.position = vec + newObject.transform.position;
        }
    }
    [PunRPC]
    void EndMove()
    {
        // update state
        used_up = true;
        LeftDoor ld = newObject.transform.GetChild(0).gameObject.GetComponent<LeftDoor>();
        ld.SetRightDoorPos(new Vector2(rightDoorObject.transform.position.x, rightDoorObject.transform.position.y));
        newObject = null;
        //effect = Instantiate(effect);
        //effect.transform.SetParent(rightDoorObject.transform);
        //effect = Instantiate(effect);
        //effect.transform.SetParent(ld.transform);
    }
}
