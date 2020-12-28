using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ArrowCtrl : MonoBehaviour
{
    private int cnt;
    SelectCharacter selection;
    //private bool ready = false;

    void Awake()
    {
        selection = FindObjectOfType<SelectCharacter>();
    }
    // Start is called before the first frame update
    void Start()
    {
        cnt = 1;
        Select();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            cnt--;
            if(cnt == 0) cnt = 4;
            Select();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            cnt++;
            if(cnt == 5) cnt = 1;
            Select();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(this.GetComponent<PlayerIsReady>().CanStart == true)
            {
                PhotonNetwork.LoadLevel("SampleScene");
            }            
        }
    }

    void Select()
    {
        switch(cnt)
        {
            case 1:
                gameObject.transform.position = new Vector3(-6.3f, 0.7f, 0);
                break;
            case 2:
                gameObject.transform.position = new Vector3(-3.1f, 0.7f, 0);
                break;
            case 3:
                gameObject.transform.position = new Vector3(2f, 3.1f, 0);
                break;
            case 4:
                gameObject.transform.position = new Vector3(6f, 0.3f, 0);
                break;
            default:
                break;
        }
        selection.character = cnt;
    }
}
