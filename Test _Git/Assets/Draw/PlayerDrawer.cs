using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDrawer : Prop
{
    public GameObject linePrefab;
    public LayerMask cantDrawOverLayer;
    int cantDrawOverLayerIndex;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance = 0.2f;
    public float lineWidth = 0.1f;
    public float limitLength = 50.0f;

    GameObject newObject;
    GameObject currentLinePrefab;
    Line currentLine;

    Camera cam;
    public bool canDraw;
    private int playerLayer;

    void Start()
    {
        cam = Camera.main;
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
        playerLayer = LayerMask.NameToLayer("Player");
        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "LimitLength", limitLength } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        canDraw = true;
    }

    void Update()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if(limitLength > 0 && canDraw)
        {
            if (Input.GetMouseButtonDown(0))
                this.photonView.RPC("BeginDrawPlayer", RpcTarget.AllBuffered);

            if (currentLine != null)
            {
                Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                this.photonView.RPC("DrawPlayer", RpcTarget.AllBuffered, mousePosition, limitLength);
                ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "LimitLength", limitLength } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            }

            if (Input.GetMouseButtonUp(0))
                this.photonView.RPC("EndDrawPlayer", RpcTarget.AllBuffered);
        }
            
    }

    // Begin Draw ----------------------------------------------
    [PunRPC]
    void BeginDrawPlayer()
    {
        if (limitLength > 0.0f)
        {
            newObject = new GameObject("LineObject");
            currentLinePrefab = (GameObject)Instantiate(linePrefab, newObject.transform);
            currentLine = currentLinePrefab.GetComponent<Line>();

            //Set line properties
            currentLine.UsePhysics(false);
            currentLine.SetLineColor(lineColor);
            currentLine.SetPointsMinDistance(linePointsMinDistance);
            currentLine.SetLineWidth(lineWidth);
        }
    }
    // Draw ----------------------------------------------------
    [PunRPC]
    void DrawPlayer(Vector2 mousePosition, float limit)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, 1<<cantDrawOverLayerIndex | 1<<playerLayer);

        limitLength = limit;
        if (hit || limitLength <= 0.0f)
            EndDrawPlayer();
        else
        {
            Line current = null;
            if(currentLinePrefab)
                currentLinePrefab.TryGetComponent<Line>(out current);
            if(current)
                limitLength -= current.AddPoint(mousePosition, true);            
        }

    }
    // End Draw ------------------------------------------------
    [PunRPC]
    void EndDrawPlayer()
    {
        if (currentLine != null)
        {
            if (currentLine.pointsCount < 2)
            {
                //If line has one point
                Destroy(currentLine.gameObject);
                Destroy(newObject);
            }
            else
            {
                // update state
                used_up = true;

                //Add the line to "CantDrawOver" layer
                currentLine.gameObject.layer = cantDrawOverLayerIndex;

                //Activate Physics on the line
                //currentLine.UsePhysics(true);

                newObject = null;
                currentLine = null;
            }
        }
    }
}
