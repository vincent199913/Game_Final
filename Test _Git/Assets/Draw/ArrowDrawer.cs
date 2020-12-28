using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArrowDrawer : Prop
{
    public GameObject linePrefab;
    public GameObject ArrowPrefab;
    public GameObject BowPrefab;
    public LayerMask cantDrawOverLayer;
    int cantDrawOverLayerIndex;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance = 0.2f;
    public float lineWidth = 0.1f;
    public float limitLength = 5.0f;

    [Space(30f)]
    public float arrowMoveSpeed = 4.0f;

    GameObject newObject;
    Line currentLine;
    Arrow currentArrow;

    Camera cam;
    private int playerLayer;

    void Start()
    {
        cam = Camera.main;
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
            this.photonView.RPC("BeginDrawArrow", RpcTarget.AllBuffered);

        if (currentLine != null)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            this.photonView.RPC("DrawArrow", RpcTarget.AllBuffered, mousePosition);
        }
            

        if (Input.GetMouseButtonUp(0))
            this.photonView.RPC("EndDrawArrow", RpcTarget.AllBuffered);
        
    }

    // Begin Draw ----------------------------------------------
    [PunRPC]
    void BeginDrawArrow()
    {
        if(limitLength > 0.0f)
        {
            newObject = new GameObject("ArrowObject");
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
    void DrawArrow(Vector2 mousePosition)
    {
        //Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, 1<<cantDrawOverLayerIndex | 1<<playerLayer);

        if (hit || limitLength <= 0.0f)
            EndDrawArrow();
        else
            limitLength -= currentLine.AddPoint(mousePosition, false);
    }
    // End Draw ------------------------------------------------
    [PunRPC]
    void EndDrawArrow()
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
                // update state
                if (limitLength < 0)
                {
                    used_up = true;
                }
                //used_up = true;
                //Add the line to "CantDrawOver" layer
                currentLine.gameObject.layer = cantDrawOverLayerIndex;

                //Activate Physics on the line
                //currentLine.UsePhysics(true);

                // Create Bow
                GameObject bowObject = new GameObject("Bow");
                bowObject.transform.position = currentLine.points[0];
                Vector2 tmp = currentLine.points[1] - currentLine.points[0];
                bowObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -Vector2.Angle(Vector2.left, tmp));
                Instantiate(BowPrefab, bowObject.transform);
                bowObject.transform.SetParent(newObject.transform);

                // Add Arrow to the line
                GameObject arrowObject = new GameObject("Arrow");
                arrowObject.transform.position = currentLine.points[0];
                currentArrow = Instantiate(ArrowPrefab, arrowObject.transform).GetComponent<Arrow>();
                currentArrow.SetArrowMoveSpeed(arrowMoveSpeed);
                currentArrow.Move(currentLine.points);
                arrowObject.transform.SetParent(newObject.transform);

                currentLine.gameObject.SetActive(false);
                newObject = null;
                currentLine = null;
            }
        }
    }
}
