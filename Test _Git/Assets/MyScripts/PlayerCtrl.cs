using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviourPun, IPunObservable
{
    // State Control
    public enum PlayerState {STAY, START}
    public PlayerState state;
    public int Signal = 0; // is used to request the controller to change the state.
    // UI
    public int lives = 3;
    //public Image live_img_left, live_img_mid, live_img_right;
    public float score = 0f;

    // Behavior Control
    public MouseCtrl mouse;
    public float moveSpeed;
    public float jumpForce;
    Animator ani;
    public Transform standPoint;
    public float standRadius;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool faceRight;

    public AudioClip AC_get;
    public AudioClip AC_win;
    public AudioClip AC_hurt;
    public AudioSource AS;

    private SpriteRenderer sprite;
    public Text nameText;
    public int myIndex;
    public bool EndLevel;

    private Vector2 InitPos;
    SelectCharacter selection;

    private int playerLayer;
    private int cantDrawLayer;
    private bool jumpOffCoroutineIsRunning;
    private float jumpTime = 0.2f;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        selection = FindObjectOfType<SelectCharacter>();
        InitPos = new Vector2 (this.transform.position.x, -1.3f);
        if (photonView.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;            
        }
        else
        {
            nameText.text = photonView.Owner.NickName;
        }
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        faceRight = true;
        ani = GetComponent<Animator>();
        isGrounded = false;
        sprite = GetComponent<SpriteRenderer>();
        if (SceneManager.GetActiveScene().name == "Canyon")
        {
            this.GetComponent<PlayerDrawer>().limitLength = 50.0f;
            this.GetComponent<PlayerDrawEraser>().enabled = true;
            mouse = GameObject.Find("Mouse").GetComponent<MouseCtrl>();
            mouse.line_drawer = gameObject.GetComponent<PlayerDrawer>();
            mouse.eraser_drawer = gameObject.GetComponent<PlayerDrawEraser>();
            myIndex = GameObject.Find("GameManager_canyon").GetComponent<GameManager_canyon>().myIndex;
            ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", lives } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            myProperty = new ExitGames.Client.Photon.Hashtable() { { "PlayerChoose", selection.character } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            myProperty = new ExitGames.Client.Photon.Hashtable() { { "LimitLength", 50.0f } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            myProperty = new ExitGames.Client.Photon.Hashtable() { { "EndLevel", false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        }
        else
        {
            state = PlayerState.START;
            this.GetComponent<PlayerDrawEraser>().enabled = false;
            this.GetComponent<PlayerDrawer>().limitLength = float.MaxValue;
            this.GetComponent<PlayerDrawer>().canDraw = true;
        }
        playerLayer = LayerMask.NameToLayer("Player");
        cantDrawLayer = LayerMask.NameToLayer("CantDrawOver");
    }
    // Start is called before the first frame update
    void Start()
    {
        /*if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        faceRight = true;
        ani = GetComponent<Animator>();
        isGrounded = false;
        sprite = GetComponent<SpriteRenderer>();
        mouse = GameObject.Find("Mouse").GetComponent<MouseCtrl>();
        mouse.line_drawer = gameObject.GetComponent<LineDrawer>();
        mouse.eraser_drawer = gameObject.GetComponent<DrawEraser>();
        myIndex = GameObject.Find("GameManager_canyon").GetComponent<GameManager_canyon>().myIndex;
        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", lives } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        //live_img_left = GameObject.Find("Mouse").*/
    }

    // Update is called once per frame
    void Update()
    {
        // General State Control
        /*if (lives >= 3) live_img_right.gameObject.SetActive(true);
        else live_img_right.gameObject.SetActive(false);
        if (lives >= 2) live_img_mid.gameObject.SetActive(true);
        else live_img_mid.gameObject.SetActive(false);
        if (lives >= 1) live_img_left.gameObject.SetActive(true);
        else live_img_left.gameObject.SetActive(false);*/
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {            
            return;
        }
        if (state == PlayerState.STAY){
            // Object Behavior...
        }
        else if (state == PlayerState.START){
            if(Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.DownArrow))
            {
                if(isGrounded)
                {
                    //float dir = faceRight? 1f : -1f;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(Input.GetAxis("Horizontal"), jumpForce));
                    ani.SetTrigger("Jump");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine("JumpOff");
                if (isGrounded)
                {
                    
                }
            }
            else if(Input.GetMouseButtonDown(0))
            {
                ani.SetBool("Draw", true);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                ani.SetBool("Draw", false);
            }

            /*if(faceRight)
            {
                if(Input.GetAxis("Horizontal") < 0)
                {
                    Flip();
                }
            }
            else if(Input.GetAxis("Horizontal") > 0)
            {
                Flip();
            }*/
        }
        else{
            Debug.Log("[PLAYER] ERROR: GAME STATE ERROR.");
        }
        if (Input.GetAxis("Horizontal") < 0)
            this.photonView.RPC("Flip", RpcTarget.AllBufferedViaServer, true);
            //sprite.flipX = true;
        if (Input.GetAxis("Horizontal") > 0)
            //sprite.flipX = false;
            this.photonView.RPC("Flip", RpcTarget.AllBufferedViaServer, false);
    }

    
    void FixedUpdate()
    {
        
        if(state == PlayerState.STAY){
            // Object Behavior...
        }
        else if (state == PlayerState.START){
            float h = Input.GetAxis("Horizontal");

            if(!isGrounded)
            {
                Debug.Log(isGrounded);
                isGrounded = Physics2D.OverlapCircle (standPoint.position, standRadius, groundLayer);
                if(isGrounded)
                {
                    //Debug.Log(isGrounded);
                    ani.SetTrigger("Land");
                }
            }

            isGrounded = Physics2D.OverlapCircle (standPoint.position, standRadius, groundLayer);
            if(isGrounded)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(h * moveSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else{
            Debug.Log("[PLAYER] ERROR: GAME STATE ERROR.");
        }
    }

    IEnumerator JumpOff()
    {
        Debug.Log("jump off----Start");
        jumpOffCoroutineIsRunning = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CapsuleCollider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().gravityScale += 5;
        //Physics2D.IgnoreLayerCollision(cantDrawLayer, cantDrawLayer, true);
        yield return new WaitForSeconds(jumpTime);
        Debug.Log("jump off----End");
        //Physics2D.IgnoreLayerCollision(cantDrawLayer, cantDrawLayer, false);
        jumpOffCoroutineIsRunning = false;
        this.GetComponent<BoxCollider2D>().enabled = true;
        this.GetComponent<CapsuleCollider2D>().enabled = true;
        this.GetComponent<Rigidbody2D>().gravityScale -= 5;
    }
    [PunRPC]
    public void Flip(bool _flip)
    {
        sprite.flipX = _flip;
    }
    void OnDrawGizmosSelected()
    {
        if(standPoint == null)
            return;
        Gizmos.DrawWireSphere(standPoint.position, standRadius);
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Weapon"){
            if (photonView.IsMine)
            {
                Debug.Log(col.gameObject.tag);
                if (lives > 0)
                    lives -= 1;
                Debug.Log("myIndex" + PhotonNetwork.LocalPlayer.ActorNumber);
                ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", lives } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                //this.photonView.RPC("MinusLives", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, lives);

                Vector3 hazard = col.gameObject.transform.position;
                Vector3 knockBackDir = ((transform.position - hazard).normalized + Vector3.up).normalized;
                GetComponent<Rigidbody2D>().AddForce(knockBackDir * 1000f);
                AS.PlayOneShot(AC_hurt);
            }
        }        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Lift")
        {
            this.photonView.RPC("SetLag", RpcTarget.AllBufferedViaServer);
        }
    }
    [PunRPC]
    public void SetLag()
    {
        this.GetComponent<LagCompensation>().enabled = true;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!this.photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (col.tag == "Destination"){
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            score += 50f;
            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:                    
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Score", score } });
                    break;
                case 2:
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P2Score", score } });
                    break;
                case 3:
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P3Score", score } });
                    break;
                case 4:
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P4Score", score } });
                    break;
                default:
                    break;
            }
            Signal = 1;
            ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "EndLevel", true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            AS.PlayOneShot(AC_win);
            Debug.Log(col.tag);
        }
        else if(col.tag == "Eraser"){
            Debug.Log(col.tag);
            gameObject.GetComponent<PlayerDrawEraser>().limitNumber = 1;
            ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            col.gameObject.SetActive(false);
            mouse.has_eraser = true;        
            AS.PlayOneShot(AC_get);
        }
    }
    public void init(PlayerState s){
        this.transform.SetParent(null);
        if (s == PlayerState.START) transform.position = InitPos;
        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);        
        myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", 3 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
    }
    public void count_score(float s){
        score += s;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
        {
            stream.SendNext(lives);
        }
        else
        {
            lives = (int)stream.ReceiveNext();
        }*/
    }
}
