using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;

public class GameManager_canyon : MonoBehaviourPun
{
    private GameObject m_player;
    public Transform[] PlayerSpawnPoint;

    Player[] myplayer;
    SelectCharacter selection;
    //public GameObject m_CM;
    public int myIndex = 0;

    void Awake()
    {
        selection = FindObjectOfType<SelectCharacter>();
        myplayer = PhotonNetwork.PlayerList;
        for (int i = 0; i < myplayer.Length; i++)
        {
            if (myplayer[i].NickName == PhotonNetwork.NickName)
            {
                myIndex = i;
                switch (selection.character)
                {
                    case 1: m_player = PhotonNetwork.Instantiate("Player Variant", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 2: m_player = PhotonNetwork.Instantiate("Player_Santa", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 3: m_player = PhotonNetwork.Instantiate("Player_Grandma", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 4: m_player = PhotonNetwork.Instantiate("Player_Statue", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    default: break;
                }
                //m_player = PhotonNetwork.Instantiate("Player Variant", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0);
                //Debug.Log("myIndex" + myIndex);
                ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "PlayerChoose", selection.character } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                GameObject m_cm = GameObject.Find("Main Camera");
                m_cm.GetComponent<CameraCtrl>().player = m_player.GetComponent<PlayerCtrl>();
                GameObject.Find("LevelController").GetComponent<LevelCtrl>().player = m_player.GetComponent<PlayerCtrl>();
                GameObject.Find("UI_canvas").GetComponent<CanvasCtrl>().player = m_player.GetComponent<PlayerCtrl>();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        /*myplayer = PhotonNetwork.PlayerList;
        for (int i = 0; i < myplayer.Length; i++)
        {
            if (myplayer[i].NickName == PhotonNetwork.NickName)
            {
                myIndex = i;
                switch (selection.character)
                {
                    case 1: m_player = PhotonNetwork.Instantiate("Player Variant", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 2: m_player = PhotonNetwork.Instantiate("Player_Santa", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 3: m_player = PhotonNetwork.Instantiate("Player_Grandma", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    case 4: m_player = PhotonNetwork.Instantiate("Player_Statue", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0); break;
                    default: break;
                }
                ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "PlayerChoose", selection.character } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                //m_player = PhotonNetwork.Instantiate("Player Variant", PlayerSpawnPoint[i].transform.position, Quaternion.identity, 0);
                //Debug.Log("myIndex" + myIndex);
                GameObject m_cm = GameObject.Find("Main Camera");
                m_cm.GetComponent<CameraCtrl>().player = m_player.GetComponent<PlayerCtrl>();
                GameObject.Find("LevelController").GetComponent<LevelCtrl>().player = m_player.GetComponent<PlayerCtrl>();
                GameObject.Find("UI_canvas").GetComponent<CanvasCtrl>().player = m_player.GetComponent<PlayerCtrl>();
            }
        }*/
    }    
}
