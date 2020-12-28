using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class GameManager : MonoBehaviourPun
{
    private GameObject m_player;
    //public GameObject m_CM;
    public CinemachineVirtualCamera m_cm;
    // Start is called before the first frame update
    void Start()
    {
        m_player = PhotonNetwork.Instantiate("Player Variant", new Vector3(-10.44f, -6.0f, 0), Quaternion.identity, 0);
        m_cm.Follow = m_player.transform;
        //Camera.main.GetComponent<ThirdP>().target = m_player;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
