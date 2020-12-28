using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCollision : MonoBehaviourPun
{
    public int lives = 3;
    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Weapon")
        {
            Debug.Log(col.gameObject.tag);
            if (lives > 0)
                lives -= 1;
            Debug.Log("myIndex" + PhotonNetwork.LocalPlayer.ActorNumber);
            //ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", lives } };
            //PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
            //this.photonView.RPC("MinusLives", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, lives);

            Vector3 hazard = col.gameObject.transform.position;
            Vector3 knockBackDir = ((transform.position - hazard).normalized + Vector3.up).normalized;
            GetComponent<Rigidbody2D>().AddForce(knockBackDir * 1000f);
            //AS.PlayOneShot(AC_hurt);
        }
    }
}
