using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EmptyObject : MonoBehaviourPun
{
    // Start is called before the first frame update
   
    [PunRPC]
    public void RenameGameObject(string name)
    {
        this.name = name;
    }
    public void SetMyParent(int child, int parent)
    {
        var thischild = PhotonView.Find(child);
        var thisparent = PhotonView.Find(parent);
        thischild.transform.SetParent(thisparent.transform);
    }
}
