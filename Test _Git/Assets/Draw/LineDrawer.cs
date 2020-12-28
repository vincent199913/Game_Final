using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LineDrawer : Prop
{
    public GameObject linePrefab;
    public LayerMask cantDrawOverLayer;
    int cantDrawOverLayerIndex;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance = 0.2f;
    public float lineWidth = 0.1f;
    public float limitLength = 5.0f;

    GameObject newObject;
    Line currentLine;

    Camera cam;


    void Start()
    {
        cam = Camera.main;
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
        //ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "LimitLength", limitLength } };
       //PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
    }

    void Update()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
            this.photonView.RPC("BeginDrawLine", RpcTarget.AllBuffered);

        if (currentLine != null)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            this.photonView.RPC("DrawLine", RpcTarget.AllBuffered, mousePosition);
        }


        if (Input.GetMouseButtonUp(0))
            this.photonView.RPC("EndDrawLine", RpcTarget.AllBuffered);
        
    }

    // Begin Draw ----------------------------------------------
    [PunRPC]
    void BeginDrawLine()
    {
        if(limitLength > 0.0f)
        {
            newObject = new GameObject("LineObject");
            currentLine = Instantiate(linePrefab, newObject.transform).GetComponent<Line>();

            //Set line properties
            currentLine.UsePhysics(false);
            currentLine.SetLineColor(lineColor);
            currentLine.SetPointsMinDistance(linePointsMinDistance);
            currentLine.SetLineWidth(lineWidth);
        }
    }
    // Draw ----------------------------------------------------
    [PunRPC]
    void DrawLine(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayer);

        if (hit || limitLength <= 0.0f)
            EndDrawLine();
        else
        {
            limitLength -= currentLine.AddPoint(mousePosition, true);
            //ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "LimitLength", limitLength } };
            //PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        }
            
    }
    // End Draw ------------------------------------------------
    [PunRPC]
    void EndDrawLine()
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
                if (limitLength < 0)
                {
                    used_up = true;
                }
                // update state
                //used_up = true;

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
