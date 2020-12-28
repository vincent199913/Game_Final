using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;

public class MouseCtrl : MonoBehaviourPun
{
    // State Control
    public enum MouseState {STAY, SELECT, DRAW, PLAY}
    public MouseState state;
    public int Signal = 0; // is used to request the controller to change state.

    // Behavior Control
    public bool has_eraser = false;
    public float stroke_left = 1f;
    public Image eraser_img;
    public Image stroke;
    public GameObject disable_icon;

    public Prop prop_1;
    public Prop prop_2;
    public Prop prop_3;
    public Prop prop_4;
    public Prop prop_5;
    public Prop prop_6;
    public Prop prop_7;
    public Prop prop_8;
    public int selected_prop_cat;

    private Prop current_prop;
    private int pipeline_idx = 0;

    // line drawer
    public PlayerDrawer line_drawer;
    public PlayerDrawEraser eraser_drawer;
    public float stroke_max = 50f;

    void create_prop(int cat){
        Prop prop;
        switch (cat){
            case 1:
                //prop = Instantiate(prop_1);
                prop = PhotonNetwork.Instantiate("prop_eraser Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                break;
            case 2:
                prop = PhotonNetwork.Instantiate("prop_gear Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_2);
                break;
            case 3:
                prop = PhotonNetwork.Instantiate("prop_line Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_3);
                break;
            case 4:
                prop = PhotonNetwork.Instantiate("prop_arraw Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_4);
                break;
            case 5:
                prop = PhotonNetwork.Instantiate("prop_door Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_5);
                break;
            case 6:
                prop = PhotonNetwork.Instantiate("prop_sticky Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_6);
                break;
            case 7:
                prop = PhotonNetwork.Instantiate("prop_lift Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_7);
                break;
            case 8:
                prop = PhotonNetwork.Instantiate("prop_fire Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_8);
                break;
            default:
                prop = PhotonNetwork.Instantiate("prop_eraser Variant", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<Prop>();
                //prop = Instantiate(prop_1);
                break;
        }
        current_prop = prop;
    }
    void Update_UI(){
        //if (has_eraser) eraser_img.gameObject.SetActive(true);
        //else eraser_img.gameObject.SetActive(false);
        //stroke.rectTransform.sizeDelta = new Vector2(100f*stroke_left, 100f);
        //stroke.rectTransform.anchoredPosition = new Vector2(-26.9f+37.4f*stroke_left, 6f);
    }

	void Update()
	{
        //Update_UI();

        // state control
        if(state == MouseState.STAY){
            // Nothing ... 
            //line_drawer.gameObject.SetActive(false);
            //eraser_drawer.gameObject.SetActive(false);
            eraser_drawer.enabled = false;
            line_drawer.enabled = false;
            disable_icon.SetActive(false);
        }
        else if (state == MouseState.SELECT){
            // Nothing ... 
            //line_drawer.gameObject.SetActive(false);
            //eraser_drawer.gameObject.SetActive(false);
            eraser_drawer.enabled = false;
            line_drawer.enabled = false;
            disable_icon.SetActive(false);
        }
        else if(state == MouseState.DRAW){
            //line_drawer.gameObject.SetActive(false);
            //eraser_drawer.gameObject.SetActive(false);
            eraser_drawer.enabled = false;
            line_drawer.enabled = false;
            if (pipeline_idx == 0){ // Create Prop
                create_prop(selected_prop_cat);
                pipeline_idx = 1;
            }
            else if(pipeline_idx == 1){ // Use Prop
                // listen event
                if(Camera.main.ScreenToWorldPoint(Input.mousePosition).y>=15.44f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y<=109.28f ){
                    current_prop.gameObject.gameObject.SetActive(true);
                    disable_icon.SetActive(false);
                }
                else{
                    current_prop.gameObject.gameObject.SetActive(false);
                    disable_icon.SetActive(true);
                    disable_icon.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                if(current_prop.used_up == true){
                    Signal = 1;
                    current_prop.used_up = false;
                    line_drawer.limitLength = stroke_max;
                }
            }
        }
        else if(state == MouseState.PLAY){
            eraser_drawer.enabled = true;
            line_drawer.enabled = true;
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y>=15.44f && Camera.main.ScreenToWorldPoint(Input.mousePosition).y<=109.28f ){
                //line_drawer.gameObject.SetActive(true);              
                
                line_drawer.GetComponent<PlayerDrawer>().canDraw = true;
                //stroke_left = line_drawer.limitLength / stroke_max;
                if (has_eraser){
                    //eraser_drawer.gameObject.SetActive(true);
                    //eraser_drawer.enabled = true;
                    eraser_drawer.GetComponent<PlayerDrawEraser>().canDraw = true;
                    //line_drawer.enabled = false;
                    //line_drawer.gameObject.SetActive(false);
                    if (eraser_drawer.limitNumber <= 0)
                    {
                        has_eraser = false;
                        ExitGames.Client.Photon.Hashtable myProperty = new ExitGames.Client.Photon.Hashtable() { { "HasEraser", false } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(myProperty);
                    }
                }
                else
                {
                    //eraser_drawer.gameObject.SetActive(false);
                    //line_drawer.gameObject.SetActive(true);
                    //eraser_drawer.enabled = false;
                    eraser_drawer.GetComponent<PlayerDrawEraser>().canDraw = false;
                    //line_drawer.GetComponent<PlayerDrawer>().canDraw = false;
                    //line_drawer.enabled = true;
                }
                disable_icon.SetActive(false);
            }
            else{
                //eraser_drawer.enabled = false;
                eraser_drawer.GetComponent<PlayerDrawEraser>().canDraw = false;
                line_drawer.GetComponent<PlayerDrawer>().canDraw = false;
                //line_drawer.enabled = false;
                //line_drawer.gameObject.gameObject.SetActive(false);
                //eraser_drawer.gameObject.gameObject.SetActive(false);
                disable_icon.SetActive(true);
                disable_icon.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
		else{
            Debug.Log("[MOUSE] ERROR: GAME STATE ERROR.");
        }
	}
    public void init(MouseState s){
        if (s == MouseState.DRAW) pipeline_idx = 0;
    }
}
