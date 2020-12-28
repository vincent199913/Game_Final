using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomListButton : MonoBehaviourPun
{
    public Text roomName;    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {       
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
