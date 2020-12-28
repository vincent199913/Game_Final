using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftDoor : MonoBehaviour
{
    public Vector2 rightDoorPos = Vector2.zero;

    public void SetRightDoorPos(Vector2 vec)
    {
        rightDoorPos = vec;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // may need to judge tag
        if(col.gameObject.tag == "Player")
            col.gameObject.transform.position = rightDoorPos;
    }
}
