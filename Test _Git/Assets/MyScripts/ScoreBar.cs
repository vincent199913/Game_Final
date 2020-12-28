using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBar : MonoBehaviour
{
    public float max_length = 30f;
    public float current_length = 0f;
    public bool locked = false;

    // Update is called once per frame
    void Update()
    {
        if (!locked){
            if(current_length < max_length){
                current_length += 1f;
                transform.position = new Vector3(transform.position.x+0.5f, transform.position.y, transform.position.z);
            }
            else{
                locked = true;
            }
        }
    }
}
