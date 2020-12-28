using UnityEngine;
using Photon.Pun;

public class LineDrawer1 : MonoBehaviourPun
{

	public GameObject linePrefab;
	public LayerMask cantDrawOverLayer;
	int cantDrawOverLayerIndex;

	[Space(30f)]
	public Gradient lineColor;
	public float linePointsMinDistance;
	public float lineWidth;
	public float limitLength = 5.0f;

	line currentLine;
	GameObject currentLinePrefab;
	PhotonView pView;
	Camera cam;


	void Start()
	{
		linePointsMinDistance = 0.2f;
		lineWidth = 0.1f;
		limitLength = 5.0f;
		cam = Camera.main;
		cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
	}

	void Update()
	{
		if (!photonView.IsMine && PhotonNetwork.IsConnected)
			return;
		if (Input.GetMouseButtonDown(0))
        {
			currentLinePrefab = PhotonNetwork.Instantiate("MYLine", /*this.transform.position*/ new Vector3(0, 0, 0), Quaternion.identity, 0);
			pView = currentLinePrefab.GetComponent<PhotonView>();
			currentLine = currentLinePrefab.GetComponent<line>();
			pView.RPC("UsePhysics", RpcTarget.All, false);			
			//pView.RPC("SetLineColor", RpcTarget.All, lineColor);
			pView.RPC("SetPointsMinDistance", RpcTarget.All, linePointsMinDistance);
			pView.RPC("SetLineWidth", RpcTarget.All, lineWidth);

			//this.photonView.RPC("BeginDraw", PhotonTargets.All);
		}

		//BeginDraw();			

		if (currentLine != null)
			this.photonView.RPC("Draw", RpcTarget.All);

		if (Input.GetMouseButtonUp(0))
			this.photonView.RPC("EndDraw", RpcTarget.All);

	}

	// Begin Draw ----------------------------------------------
	[PunRPC]
	void BeginDraw()
	{		
		//currentLinePrefab = Instantiate(linePrefab, this.transform.position, Quaternion.identity);
		currentLine = currentLinePrefab.GetComponent<line>();
		pView = currentLinePrefab.GetComponent<PhotonView>();
		//Set line properties
		pView.RPC("UsePhysics", RpcTarget.All, false);
		//currentLine.UsePhysics(false);
		pView.RPC("SetLineColor", RpcTarget.All, lineColor);
		//currentLine.SetLineColor(lineColor);
		pView.RPC("SetPointsMinDistance", RpcTarget.All, linePointsMinDistance);
		//currentLine.SetPointsMinDistance(linePointsMinDistance);
		pView.RPC("SetLineWidth", RpcTarget.All, lineWidth);
		//currentLine.SetLineWidth(lineWidth);

	}
	// Draw ----------------------------------------------------
	[PunRPC]
	void Draw()
	{
		Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

		//Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
		RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayer);
		
		if (hit)
		{
			this.photonView.RPC("EndDraw", RpcTarget.All);
		}
        else
		{		
			if (pView)				
				pView.RPC("AddPoint", RpcTarget.All, mousePosition);
			//currentLine.AddPoint(mousePosition);
		}

	}
	// End Draw ------------------------------------------------

	[PunRPC]
	void EndDraw()
	{
		if (currentLinePrefab != null)
		{
			if (currentLine.pointsCount < 2)
			{
				//If line has one point
				PhotonNetwork.Destroy(currentLinePrefab);
				//PhotonNetwork.Destroy(currentLine.gameObject);
			}
			else
			{
				//Add the line to "CantDrawOver" layer
				currentLinePrefab.gameObject.layer = cantDrawOverLayerIndex;

				//Activate Physics on the line
				//currentLine.UsePhysics(true);

				currentLine = null;
			}
		}
	}

    
}