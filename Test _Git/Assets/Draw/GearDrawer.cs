using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GearDrawer : Prop
{
    public GameObject linePrefab;
    public GameObject gearPrefab;
    public LayerMask cantDrawOverLayer;
    int cantDrawOverLayerIndex;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance = 0.2f;
    public float lineWidth = 0.1f;
    public float limitLength = 5.0f;

    [Space(30f)]
    public float gearMoveSpeed = 4.0f;
    public float gearRotationSpeed = 20.0f;

    GameObject newObject;
    Line currentLine;
    Gear currentGear;

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
            this.photonView.RPC("BeginDrawGear", RpcTarget.AllBuffered);

        if (currentLine != null)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            this.photonView.RPC("DrawGear", RpcTarget.AllBuffered, mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
            this.photonView.RPC("EndDrawGear", RpcTarget.AllBuffered);
        
    }

    // Begin Draw ----------------------------------------------
    [PunRPC]
    void BeginDrawGear()
    {
        if(limitLength > 0.0f)
        {
            newObject = new GameObject("GearObject");
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
    void DrawGear(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayer);

        if (hit || limitLength <= 0.0f)
            EndDrawGear();
        else
            limitLength -= currentLine.AddPoint(mousePosition, true);
    }
    // End Draw ------------------------------------------------
    [PunRPC]
    void EndDrawGear()
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
                //currentLine.UsePhysics(true);
                currentLine.GetComponent<EdgeCollider2D>().isTrigger = true;

                // Add Gear to the line
                GameObject gearObject = new GameObject("Gear");
                gearObject.transform.position = currentLine.points[0];
                currentGear = Instantiate(gearPrefab, gearObject.transform).GetComponent<Gear>();
                currentGear.SetGearMoveSpeed(gearMoveSpeed);
                currentGear.SetGearRotationSpeed(gearRotationSpeed);
                currentGear.Move(currentLine.points);
                gearObject.transform.SetParent(newObject.transform);

                newObject = null;
                currentLine = null;
            }
        }
    }
}
