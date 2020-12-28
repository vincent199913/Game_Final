using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestGameManager : MonoBehaviourPun
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("prop_gear Variant", new Vector3(0, 0, 0), Quaternion.identity, 0);
        //Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
