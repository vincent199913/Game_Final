using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LiftDrawer : Prop
{
    public GameObject linePrefab;
    public GameObject LiftPrefab;
    public LayerMask cantDrawOverLayer;
    int cantDrawOverLayerIndex;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance = 0.2f;
    public float lineWidth = 0.1f;
    public float limitLength = 5.0f;

    [Space(30f)]
    public float liftMoveSpeed = 4.0f;

    GameObject newObject;
    Line currentLine;
    Lift currentLift;

    Camera cam;


    void Start()
    {
        cam = Camera.main;
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
    }

    void Update()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
            this.photonView.RPC("BeginDrawLift", RpcTarget.AllBuffered);

        if (currentLine != null)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            this.photonView.RPC("DrawLift", RpcTarget.AllBuffered, mousePosition);
        }


        if (Input.GetMouseButtonUp(0))
            this.photonView.RPC("EndDrawLift", RpcTarget.AllBuffered);
        
    }

    // Begin Draw ----------------------------------------------
    [PunRPC]
    void BeginDrawLift()
    {
        if(limitLength > 0.0f)
        {
            newObject = new GameObject("LiftObject");
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
    void DrawLift(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayer);

        if (hit || limitLength <= 0.0f)
            EndDrawLift();
        else
            limitLength -= currentLine.AddPoint(mousePosition, false);
    }
    // End Draw ------------------------------------------------
    [PunRPC]
    void EndDrawLift()
    {
        if (currentLine != null)
        {
            if (currentLine.pointsCount < 2)
            {
                //If line has one point
                Destroy(currentLine.gameObject);
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
                //currentLine.UsePhysics(false);
                currentLine.GetComponent<EdgeCollider2D>().isTrigger = true;

                // Add Lift to the line
                GameObject liftObject = new GameObject("Lift");
                liftObject.transform.position = currentLine.points[0];
                currentLift = Instantiate(LiftPrefab, liftObject.transform).GetComponent<Lift>();
                currentLift.SetLiftMoveSpeed(liftMoveSpeed);
                currentLift.Move(currentLine.points);
                liftObject.transform.SetParent(newObject.transform);
                newObject = null;
                currentLine = null;
            }
        }
    }
}
