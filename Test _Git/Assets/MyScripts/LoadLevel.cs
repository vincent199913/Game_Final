using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoadLevel : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadCanyon()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Canyon");
    }

    public void loadCity()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("City");
    }

    public void loadCharSelect()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("SelectCharacter");
    }
}
