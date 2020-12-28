using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CanvasCtrl : MonoBehaviourPunCallbacks
{
    // State Control
    public enum CanvasState {START_ANIMATION, STAY, DRAW_BAG, DRAW_GAME_UI, DRAW_SCORE}
    public CanvasState state;
    public int Signal = 0; // is used to request the controller to change state.

    // Behavior Control
    public PlayerCtrl player;
    public GameObject Level;
    public Image [] score_bar;
    public Image veil;
    public Image veil_white;
    public Image sun;
    public GameObject end;
    // Prop System
    public int Total_prop = 8;
    public int Num_prop = 4;
    public float Offset_prop = 60f;
    public PropImage prop_1;
    public PropImage prop_2;
    public PropImage prop_3;
    public PropImage prop_4;
    public PropImage prop_5;
    public PropImage prop_6;
    public PropImage prop_7;
    public PropImage prop_8;
    public int prop_selection;
    private int selected_prop_cat = 0;
    private List<PropImage> prop_list = new List<PropImage>();
    // Score Bars
    public GameObject score_board;
    public float max_score_length = 470f;
    [SerializeField]private float [] current_score = new float[4];
    // Audio
    public AudioClip AC_long;
    public AudioClip AC_short;
    public AudioSource AS;
    public AudioSource AS_surrounding;
    // All
    private int pipeline_idx = 0; // count sub-states
    private int counter = 0;
    private bool game_over = false;
    private bool next_enable = false;
    private bool AS_played = false;
    private int EndLevelPlayerIndex = -1;
    private float NowScore;
    
    public void SetEnableFalse()
    {
        //Debug.Log("Enable" + next_enable);
        if (next_enable && PhotonNetwork.IsMasterClient)
            this.photonView.RPC("NextPlay", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void NextPlay()
    {
        if (next_enable)
        {
            score_board.gameObject.SetActive(false);
            pipeline_idx = 3;
        }
    }
    PropImage create_prop(int idx, int pos_idx){
        PropImage prop;

        switch (idx){
        case 1:
            prop = Instantiate(prop_1);
            break;
        case 2:
            prop = Instantiate(prop_2);
            break;
        case 3:
            prop = Instantiate(prop_3);
            break;
        case 4:
            prop = Instantiate(prop_4);
            break;
        case 5:
            prop = Instantiate(prop_5);
            break;
        case 6:
            prop = Instantiate(prop_6);
            break;
        case 7:
            prop = Instantiate(prop_7);
            break;
        case 8:
            prop = Instantiate(prop_8);
            break;
        default:
            prop = Instantiate(prop_1);
            break;
        }
        prop.transform.SetParent(this.transform, false);
        
        if (pos_idx == 0) prop.transform.position = new Vector3(Screen.width/2+Offset_prop, Screen.height/2+Offset_prop, 0f);
        else if (pos_idx == 1) prop.transform.position = new Vector3(Screen.width/2+Offset_prop, Screen.height/2-Offset_prop, 0f);
        else if (pos_idx == 2) prop.transform.position = new Vector3(Screen.width/2-Offset_prop, Screen.height/2+Offset_prop, 0f);
        else if (pos_idx == 3) prop.transform.position = new Vector3(Screen.width/2-Offset_prop, Screen.height/2-Offset_prop, 0f);

        return prop;
    }
    
    void random_selection(int total, int num){
        List<int> list = new List<int>();
        for (int n = 1; n <= total; n++){
            list.Add(n);
        }
        for (int i = 0; i < (total-num); i++){
            int index = Random.Range(0, list.Count - 1);    //  Pick random element from the list
            list.RemoveAt(index);   //  Remove chosen element
        }
        int pos_idx = 0;
        foreach(int i in list){
            PropImage prop = create_prop(i, pos_idx++);
            prop_list.Add(prop);
        }
    }
    int examine_selection(){
        foreach(PropImage p in prop_list){
            if(p.clicked == true){
                return p.prop_cat;
            }
        }
        return -1;
    }
    bool veil_alpha(bool dark){
        if(dark){
            // Get Darker
            var tempColor = veil.color;
            tempColor.a = (veil.color.a >= 0.6f)? veil.color.a : veil.color.a+0.3f*Time.deltaTime;
            veil.color = tempColor;
            if (veil.color.a >= 0.6f) return true;
        }
        else{
            var tempColor = veil.color;
            tempColor.a = (veil.color.a <= 0.0f)? veil.color.a : veil.color.a-0.3f*Time.deltaTime;
            veil.color = tempColor;
            if (veil.color.a <= 0.0f) return true;
        }
        return false;
    }
    bool veil_white_alpha(bool dark){
        if(dark){
            // Get Darker
            var tempColor = veil_white.color;
            tempColor.a = (veil_white.color.a >= 0.99f)? veil_white.color.a : veil_white.color.a+1.6f*Time.deltaTime;
            veil_white.color = tempColor;
            if (veil_white.color.a >= 0.95f) return true;
        }
        else{
            var tempColor = veil_white.color;
            tempColor.a = (veil_white.color.a <= 0.0f)? veil_white.color.a : veil_white.color.a-1.6f*Time.deltaTime;
            veil_white.color = tempColor;
            if (veil_white.color.a <= 0.0f) return true;
        }
        return false;
    }
    bool sun_alpha(bool dark){
        if(dark){
            // Get Darker
            var tempColor = sun.color;
            tempColor.a = (sun.color.a >= 0.99f)? sun.color.a : sun.color.a+1.6f*Time.deltaTime;
            sun.color = tempColor;
            if (sun.color.a >= 0.9f) return true;
        }
        else{
            var tempColor = sun.color;
            tempColor.a = (sun.color.a <= 0.0f)? sun.color.a : sun.color.a-1.6f*Time.deltaTime;
            sun.color = tempColor;
            if (sun.color.a <= 0.0f) return true;
        }
        return false;
    }
    // (#) -----------------------------------------------------------------
    void Update()
    {
        if(state == CanvasState.STAY){
            
        }
        else if(state == CanvasState.START_ANIMATION){
            if (pipeline_idx == 0){
                if (AS_played == false){
                    AS_surrounding.Play();
                    AS_played = true;
                }
                AS_surrounding.volume -= 0.5f*Time.deltaTime;;
                bool finish = veil_alpha(true);
                if (finish){
                    pipeline_idx = 1;
                    AS.PlayOneShot(AC_short);
                }
            }
            else if (pipeline_idx == 1){// sun bright
                bool finish = sun_alpha(true);
                if (finish){
                    pipeline_idx = 2;
                    AS.PlayOneShot(AC_long);
                }
            }
            else if (pipeline_idx == 2){// sun red
                bool finish = veil_white_alpha(true);
                if (finish){
                    pipeline_idx = 3;
                    end.SetActive(true);
                    Debug.Log("Pop");
                }
            }
            else if (pipeline_idx == 3){// veil bright
                bool finish_veil1 = veil_alpha(false);
                bool finish_veil2 = veil_white_alpha(false);
                bool finish_sun = sun_alpha(false);
                if (finish_veil1 && finish_veil2 && finish_sun){
                    pipeline_idx = 0;
                    Signal = 1;
                }
            }
        }
        if(state == CanvasState.DRAW_BAG){
            if (pipeline_idx == 0){
                bool finish = veil_alpha(true);
                if (finish){
                    pipeline_idx = 1;
                }
            }
            else if (pipeline_idx == 1){ // random selection process
                random_selection(Total_prop, Num_prop);
                pipeline_idx = 2;
            }
            else if (pipeline_idx == 2){ // listen click event
                selected_prop_cat = -1;
                selected_prop_cat = examine_selection();
                if(selected_prop_cat != -1){
                    pipeline_idx = 3;
                }
            }
            else if(pipeline_idx == 3){
                counter ++;
                if(counter == 30){
                    pipeline_idx = 4;
                    counter = 0;
                    foreach(PropImage p in prop_list){
                        //p.clicked = false;
                        Destroy(p.gameObject);
                    }
                }
            }
            else if(pipeline_idx == 4){
                bool finish = veil_alpha(false);
                if (finish){
                    pipeline_idx = 0;
                    prop_list.Clear();
                    Signal = selected_prop_cat;
                }
            }
        }
        else if(state == CanvasState.DRAW_GAME_UI){
            // ...
        }
        else if(state == CanvasState.DRAW_SCORE){
            if (pipeline_idx == 0){
                bool finish = veil_alpha(true);
                if (finish){
                    pipeline_idx = 1;
                }
            }
            else if(pipeline_idx == 1){
                // Draw
                //current_score[EndLevelPlayerIndex-1] = 0f;
                score_board.gameObject.SetActive(true);
                pipeline_idx = 2;
            }
            else if(pipeline_idx == 2){// 42-276
                // Wait
                /*if (current_score[EndLevelPlayerIndex-1] < player.score) current_score[EndLevelPlayerIndex-1] += 80f*Time.deltaTime;
                else next_enable = true;
                float current_score_length = max_score_length * Mathf.Min((current_score[EndLevelPlayerIndex-1] / 100f), 1f);
                score_bar[EndLevelPlayerIndex-1].rectTransform.sizeDelta = new Vector2(current_score_length, 35f);
                //score_bar[EndLevelPlayerIndex - 1].rectTransform.anchoredPosition = new Vector2(42f+current_score_length/2f, -11f);
                if (current_score_length >= max_score_length) game_over = true;*/
                switch (EndLevelPlayerIndex)
                {
                    case 1:
                        NowScore = (float)PhotonNetwork.CurrentRoom.CustomProperties["P1Score"];
                        ///PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "P1Score", NowScore } });
                        break;
                    case 2:
                        NowScore = (float)PhotonNetwork.CurrentRoom.CustomProperties["P2Score"];
                        break;
                    case 3:
                        NowScore = (float)PhotonNetwork.CurrentRoom.CustomProperties["P3Score"];
                        break;
                    case 4:
                        NowScore = (float)PhotonNetwork.CurrentRoom.CustomProperties["P4Score"];
                        break;
                    default:
                        break;
                }
                if (current_score[EndLevelPlayerIndex - 1] < NowScore) current_score[EndLevelPlayerIndex - 1] += 50f * Time.deltaTime;
                else next_enable = true;

                score_bar[EndLevelPlayerIndex - 1].rectTransform.sizeDelta = new Vector2(current_score[EndLevelPlayerIndex - 1], 35f);
            }
            else if (pipeline_idx == 3){
                
                if (game_over == true){
                    pipeline_idx = 0;
                    next_enable = false;
                    Signal = 2;
                }
                
                bool finish = veil_alpha(false);
                Debug.Log("Fin" + finish);
                if (finish){
                    pipeline_idx = 0; 
                    next_enable = false;
                    Signal = 1;
                }
            }
        }
        else{
            //Debug.Log("[CANVAS] ERROR: GAME STATE ERROR.");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("EndLevel"))
        {
            if((bool)targetPlayer.CustomProperties["EndLevel"] == true)
            {                
                state = CanvasState.DRAW_SCORE;
                Level.GetComponent<LevelCtrl>().state = LevelCtrl.GameState.END_PLAY;
                Level.GetComponent<LevelCtrl>().call_init = true;
                pipeline_idx = 1;
                EndLevelPlayerIndex = targetPlayer.ActorNumber;
                Debug.Log("Player" + EndLevelPlayerIndex + " End");
            }
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        
        
    }
}
