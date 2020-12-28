using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    // State Control
    public enum CameraState {TO_PLAYER, FOLLOW_PLAYER, FOLLOW_MOUSE, STAY}
    public CameraState state;
    public int Signal = 0; // is used to request the controller to change state.

    // Behavior Control
    public PlayerCtrl player;
    public MouseCtrl mouse;
    public Camera camera;
    public GameObject destination;
    public float speed = 0.1f;
    public float screen_div = 0.1f;
    public int start_duration = 0;
    public float min_x = -26f;
    public float max_x = -4f;
    public float min_y = 1f;
    public float max_y = 6f;
    public float zoom_speed = 1f;

    private Vector2 destination_pos;
    private int pipeline_idx = 0; // count sub-states
    private int counter = 0;

    void Camera_Follow_Destination(float factor, bool normalized){
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 displacement = destination_pos - position;

        float dx = (normalized)? speed*factor*Time.deltaTime*displacement.normalized.x : speed*factor*Time.deltaTime*displacement.x;
        float dy = (normalized)? speed*factor*Time.deltaTime*displacement.normalized.y : speed*factor*Time.deltaTime*displacement.y;
        float x = transform.position.x+dx;
        float y = transform.position.y+dy;
        float z = transform.position.z;
        if (x < max_x && x > min_x && y < max_y && y > min_y){
            transform.position = new Vector3(x, y, z);
        }
        else if (x < max_x && x > min_x){
            transform.position = new Vector3(x, position.y, z);
        }
        else if (y < max_y && y > min_y){
            transform.position = new Vector3(position.x, y, z);
        }
    }
    void Camera_Follow_Mouse(){
        Vector2 position = Input.mousePosition;
        int x_dir = 0;
        int y_dir = 0;
        if(position.x >= Screen.width*(1-screen_div)) x_dir = 1;
        if(position.x <= Screen.width*screen_div) x_dir = -1;
        if(position.y >= Screen.height*(1-screen_div)) y_dir = 1;
        if(position.y <= Screen.height*screen_div) y_dir = -1;
        float x = transform.position.x + speed*2.6f*Time.deltaTime*x_dir;
        float y = transform.position.y + speed*6f*Time.deltaTime*y_dir;
        float z = transform.position.z;
        if (x < max_x && x > min_x && y < max_y && y > min_y){
            transform.position = new Vector3(x, y, z);
        }
        else if (x < max_x && x > min_x){
            transform.position = new Vector3(x, transform.position.y, z);
        }
        else if (y < max_y && y > min_y){
            transform.position = new Vector3(transform.position.x, y, z);
        }
    }

    void Update()
    {
        if(state == CameraState.TO_PLAYER){
            if (pipeline_idx == 0){
                counter++;
                if(counter == start_duration){
                    pipeline_idx = 1;
                    counter = 0;
                }
            }
            else if(pipeline_idx == 1){
                destination_pos = new Vector2(-25.99996f, 1.024967f);
                Camera_Follow_Destination(15f, true);
                Vector2 position = new Vector2(transform.position.x, transform.position.y);
                Vector2 displacement = destination_pos - position;
                if (displacement.magnitude <= 1f){
                    pipeline_idx = 0;
                    Signal = 1;
                }
            }
        }
        if(state == CameraState.FOLLOW_PLAYER){
            destination_pos = new Vector2(player.transform.position.x, player.transform.position.y);
            Camera_Follow_Destination(3f, false);
        }
        else if(state == CameraState.FOLLOW_MOUSE){
            Camera_Follow_Mouse();
        }
        else if(state == CameraState.STAY){
            
        }
        else{
            //Debug.Log("[CAMERA] ERROR: GAME STATE ERROR.");
        }
    }
}
