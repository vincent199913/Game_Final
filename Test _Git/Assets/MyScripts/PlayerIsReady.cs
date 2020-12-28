using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerIsReady : MonoBehaviourPunCallbacks
{
    private bool IsReady = false;
    public bool CanStart = false;
    public Text m_button;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        IsReady = false;
        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "IsPlayerReady", IsReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
        if (IsReady)
        {
            m_button.text = "Ready!";
        }
        else
        {
            m_button.text = "Not Ready?";
        }
    }



    // Update is called once per frame
    void Update()
    {        
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var photonplayer in PhotonNetwork.PlayerList)
            {
                object _isPlayerReady;
                photonplayer.CustomProperties.TryGetValue("IsPlayerReady", out _isPlayerReady);
                if (_isPlayerReady != null)
                {
                    if ((bool)_isPlayerReady == false)
                    {
                        CanStart = false;
                        return;
                    }
                }

            }
            CanStart = true;
        }        
    }
    public void SetReady()
    {
        IsReady = !IsReady;
        if (IsReady)
        {
            m_button.text = "Ready!";
        }
        else
        {
            m_button.text = "Not Ready?";
        }
        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "IsPlayerReady", IsReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
    } 
}
