using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class SpawnPlayer : MonoBehaviourPun
{
    //public GameObject player;
    //public GameObject santa;
    //public GameObject statue;
    //public GameObject grandma;

    public Vector3 startPos;
    private GameObject m_player;
    public CinemachineVirtualCamera m_cm;

    SelectCharacter selection;
    // Start is called before the first frame update
    void Awake()
    {
        selection = FindObjectOfType<SelectCharacter>();
    }

    void Start()
    {
        //startPos = new Vector3(-0.2f, 0.11f, 0);
        switch(selection.character)
        {
            case 1: m_player = PhotonNetwork.Instantiate("Player Variant", startPos, Quaternion.identity, 0); break;
            case 2: m_player = PhotonNetwork.Instantiate("Player_Santa", startPos, Quaternion.identity, 0); break;
            case 3: m_player = PhotonNetwork.Instantiate("Player_Grandma", startPos, Quaternion.identity, 0); break;
            case 4: m_player = PhotonNetwork.Instantiate("Player_Statue", startPos, Quaternion.identity, 0); break;
            default: break;
        }
        m_cm.Follow = m_player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
