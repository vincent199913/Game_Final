using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LevelCtrl : MonoBehaviourPunCallbacks
{
    // GameState: record the recent game state.
    public enum GameState { START_ANIMATION, TO_PLAYER, SELECT_PROP, PLACE_PROP, START_PLAY, END_PLAY, END_GAME}
    public GameState state;

    // Public Game Objects
    public PlayerCtrl player;
    public MouseCtrl mouse;
    public CanvasCtrl canvas;
    public CameraCtrl camera;
    public GameObject dynamic_object;
    public float added_score = 20f;

    public AudioSource AS_BGM;
    private bool EndLevel = false;

    public bool call_init = false;
    // Start is called before the first frame update
    void Start()
    {
        state = GameState.START_ANIMATION;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == GameState.START_ANIMATION){
            player.state = PlayerCtrl.PlayerState.STAY;
            mouse.state = MouseCtrl.MouseState.STAY;
            canvas.state = CanvasCtrl.CanvasState.START_ANIMATION;
            camera.state = CameraCtrl.CameraState.STAY;
            if(canvas.Signal != 0){
                canvas.Signal = 0;
                AS_BGM.Play(0);
                state = GameState.TO_PLAYER;
            }
        }
        else if(state == GameState.TO_PLAYER){
            player.state = PlayerCtrl.PlayerState.STAY;
            mouse.state = MouseCtrl.MouseState.STAY;
            canvas.state = CanvasCtrl.CanvasState.STAY;
            camera.state = CameraCtrl.CameraState.TO_PLAYER;
            if(camera.Signal != 0){
                camera.Signal = 0;
                state = GameState.SELECT_PROP;
            }
        }
        else if(state == GameState.SELECT_PROP){
            player.state = PlayerCtrl.PlayerState.STAY;
            mouse.state = MouseCtrl.MouseState.SELECT;
            canvas.state = CanvasCtrl.CanvasState.DRAW_BAG;
            camera.state = CameraCtrl.CameraState.STAY;
            if(canvas.Signal != 0){
                mouse.selected_prop_cat = canvas.Signal;
                canvas.Signal = 0;
                state = GameState.PLACE_PROP;
            }
        }
        else if(state == GameState.PLACE_PROP){
            player.state = PlayerCtrl.PlayerState.STAY;
            mouse.state = MouseCtrl.MouseState.DRAW;
            canvas.state = CanvasCtrl.CanvasState.DRAW_GAME_UI;
            camera.state = CameraCtrl.CameraState.FOLLOW_MOUSE;
            if(mouse.Signal != 0){
                mouse.Signal = 0;
                state = GameState.START_PLAY;
                call_init = true;
            }
        }
        else if(state == GameState.START_PLAY){
            // Preprocessing
            if(call_init){
                mouse.init(MouseCtrl.MouseState.DRAW);
                call_init = false;
            }
            // Processing
            player.state = PlayerCtrl.PlayerState.START;
            mouse.state = MouseCtrl.MouseState.PLAY;
            canvas.state = CanvasCtrl.CanvasState.DRAW_GAME_UI;
            camera.state = CameraCtrl.CameraState.FOLLOW_PLAYER;
            // Postprocessing
            if (player.Signal != 0){
                player.Signal = 0;
                state = GameState.END_PLAY;
                call_init = true;
            }          
            
        }
        else if(state == GameState.END_PLAY){
            // Preprocessing
            if(call_init){
                //player.count_score(added_score);
                player.init(PlayerCtrl.PlayerState.START);
                call_init = false;
            }
            player.init(PlayerCtrl.PlayerState.START);
            player.state = PlayerCtrl.PlayerState.STAY;
            mouse.state = MouseCtrl.MouseState.STAY;
            canvas.state = CanvasCtrl.CanvasState.DRAW_SCORE;
            camera.state = CameraCtrl.CameraState.STAY;
            if(canvas.Signal == 1 /*call_init*/){
                canvas.Signal = 0;
                mouse.has_eraser = false;
                ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                //mouse.stroke_left = 1f;
                player.lives = 3;
                myProperty = new ExitGames.Client.Photon.Hashtable() { { "MyLives", 3 } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                for (int i = 0; i < dynamic_object.transform.childCount; ++i){
                    dynamic_object.transform.GetChild(i).gameObject.SetActive(true);
                }

                state = GameState.TO_PLAYER;
            }
            else if(canvas.Signal == 2){
                SceneManager.LoadScene(0);
            }
        }
        else{
            Debug.Log("ERROR: GAME STATE ERROR.");
        }
    }/*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.CustomProperties.ContainsKey("EndLevel"))
        {
            if ((bool)targetPlayer.CustomProperties.ContainsKey("EndLevel") == true)
            {
                state = CanvasState.DRAW_SCORE;
                EndLevelPlayerIndex = targetPlayer.ActorNumber;
            }
        }
    }*/
}
