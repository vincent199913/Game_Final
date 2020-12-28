using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject startConnectPanel;
    public GameObject lobbyPanel;
    public GameObject warningText;
    //public GameObject roomPlayerPanel;
    public GameObject roomPanel;
    public InputField inputName;
    //public InputField roomName;

    //public GameObject roomManager;

    private string m_playerName = "Player";
    //private RoomManager m_roomManager = null;

    void Awake()
    {
        //PhotonNetwork.autoJoinLobby = true;
       // PhotonNetwork.logLevel = PhotonLogLevel.Full;
          PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        //m_roomManager = roomManager.GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void connect()
    {
        // Get Player Name from Input and then set player name
        m_playerName = startConnectPanel.GetComponentInChildren<InputField>().text;
        if (inputName.text.Length > 0) 
        {
            PhotonNetwork.NickName = inputName.text;

            // Set Application version of program
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Enter Name");            
            StartCoroutine(Wait(1));            
        }      
        
    }
    IEnumerator Wait(float _time)
    {
        warningText.SetActive(true);
        yield return new WaitForSeconds(_time);
        warningText.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        startConnectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }
    /*public override void OnJoinedLobby()
    {
        // Change interface to Lobby Panel
        startConnectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }*/

    public void disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        startConnectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }
    /*public override void OnDisconnectedFromPhoton()
    {
        // Change interface to Start Connect Panel
        startConnectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }*/
    public void createMatch()
    {
        /*if (roomName.text.Length < 2) 
            return;*/
        RoomOptions options = new RoomOptions();
        ExitGames.Client.Photon.Hashtable RoomProps = new ExitGames.Client.Photon.Hashtable();
        RoomProps.Add("P1Score", 0);
        RoomProps.Add("P2Score", 0);
        RoomProps.Add("P3Score", 0);
        RoomProps.Add("P4Score", 0);
        options.CustomRoomProperties = RoomProps;
        options.MaxPlayers = 4;
        options.EmptyRoomTtl = 0;
        string roomName = m_playerName + "'s Room";
        if (!PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default))
        {
            // The Room Name is already existed
        }
    }
    public override void OnJoinedRoom()
    {
        // Change to Room Panel
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        //roomPlayerPanel.SetActive(true);

        //this.updatePlayerList();
    }
    public void quickMatch()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.createMatch();
    }
    /*public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        // Create a room
        this.createMatch();
    }*/
    public void startMatch()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel("Canyon");
            PhotonNetwork.LoadLevel("SampleScene");
            //PhotonNetwork.LoadLevel("TestScene");
        }
    }
}
