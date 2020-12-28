using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Player_UI : MonoBehaviourPunCallbacks
{
    public GameObject[] playerInfo;
    public Sprite[] playerIcon;
    public GameObject[] playerScore;
    //public Text [] nameText;
    Player[] myplayer;
    // Start is called before the first frame update
    void Start()
    {
        myplayer = PhotonNetwork.PlayerList;
        for (int i = 0; i < myplayer.Length; i++)
        {
            this.photonView.RPC("SetName", RpcTarget.AllBuffered, myplayer[i].NickName, i);
            //this.photonView.RPC("SetIcon", RpcTarget.AllBuffered, i);
        }
        for(int i = myplayer.Length; i<playerInfo.Length; i++)
        {
            this.photonView.RPC("UnusedInfo", RpcTarget.AllBuffered, i);
        }
        //this.photonView.RPC("SetIcon", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void SetName(string playername, int _index)
    {
        playerInfo[_index].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = playername;
        playerScore[_index].transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = playername;
        //nameText[_index].text = playername;
    }    
    [PunRPC]
    public void UnusedInfo(int _index)
    {
        playerInfo[_index].SetActive(false);
        playerScore[_index].SetActive(false);


    }    
    public void SetIcon(int _index)
    {
        int choose = (int)PhotonNetwork.PlayerList[_index].CustomProperties["PlayerChoose"];
        playerScore[_index].transform.GetChild(2).gameObject.GetComponent<Image>().sprite = playerIcon[choose-  1];
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object lives;
        targetPlayer.CustomProperties.TryGetValue("MyLives",out lives);
        if (lives != null)
        {
            if((int)lives <= 3)
            {
                GameObject Blood = playerInfo[targetPlayer.ActorNumber - 1].transform.GetChild(0).transform.GetChild(2).gameObject;
                for(int i = 0; i<(int)lives; i++)
                {
                    Blood.transform.GetChild(i + 1).gameObject.SetActive(true);
                }
                for(int i=(int)lives; i<3; i++)
                {
                    Blood.transform.GetChild(i + 1).gameObject.SetActive(false);
                }
                
            }            
        }
        object HasEraser;
        targetPlayer.CustomProperties.TryGetValue("HasEraser", out HasEraser);
        if(HasEraser != null)
            playerInfo[targetPlayer.ActorNumber - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).gameObject.SetActive((bool)HasEraser);

        object playerChoose;
        targetPlayer.CustomProperties.TryGetValue("PlayerChoose", out playerChoose);
        if (playerChoose != null)
        {
            playerInfo[targetPlayer.ActorNumber - 1].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcon[(int)playerChoose - 1];
            playerScore[targetPlayer.ActorNumber - 1].transform.GetChild(2).gameObject.GetComponent<Image>().sprite = playerIcon[(int)playerChoose - 1];
        }
            
        if (targetPlayer.CustomProperties.ContainsKey("LimitLength"))
        {
            float length;
            length = (float)targetPlayer.CustomProperties["LimitLength"] < 0 ? 0 : (float)targetPlayer.CustomProperties["LimitLength"];
            playerInfo[targetPlayer.ActorNumber - 1].transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Image>().rectTransform.sizeDelta 
                = new Vector2(100f* length / 50.0f, 100f);
        }
    }

}
