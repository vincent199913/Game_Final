using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControlPermission : MonoBehaviourPun//, IPunObservable
{
    Vector3 position;
    Quaternion rotation;
    float smoothing = 10f;
    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //print ("writing");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            //print ("reciving");
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();

        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        PhotonView ph = gameObject.GetComponent<PhotonView>();
        if (ph != null)
        {
            if (!ph.IsMine && PhotonNetwork.IsConnected)
            {
                //this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                //Destroy(this.gameObject.GetComponent<Rigidbody2D>());
                //gameObject.GetComponent<PlayerCtrl>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
        }
        

    }*/
}
