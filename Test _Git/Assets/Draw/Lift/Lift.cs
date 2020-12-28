using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    float moveSpeed = 5.0f;

    public bool OnPlayer = false;
    bool isActivated = false;
    List<Vector2> points;
    int nowPoint = 0;
    int direct = 1;
    List<Transform> mplayer;

    private void Awake()
    {
        OnPlayer = false;
        mplayer = new List<Transform>();
    }
    void FixedUpdate()
    {
        if(isActivated)
        {
            if(nowPoint == points.Count - 1)
                direct = -1;
            else if(nowPoint == 0)
                direct = 1;

            // move distance
            Vector2 dist = 0.5f * (points[nowPoint + direct] - points[nowPoint]).normalized * moveSpeed * Time.deltaTime;
            // judge
            Vector2 nowPos = new Vector2(transform.position.x, transform.position.y);
            if(dist.magnitude <= (points[nowPoint + direct] - nowPos).magnitude) {
                transform.position += new Vector3(dist.x, dist.y, 0);
            }
            else {
                nowPoint += direct;
                transform.position = new Vector3(points[nowPoint].x, points[nowPoint].y, 0);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            OnPlayer = true;
            var target = c.gameObject.transform;
            c.gameObject.GetComponent<LagCompensation>().enabled = false;
            target.SetParent(this.transform);
        }
    }
    private void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            OnPlayer = true;
        }
    }
    void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            OnPlayer = false;
            var target = c.gameObject.transform;
            c.gameObject.GetComponent<LagCompensation>().enabled = true;
            //var original = target.GetComponent<TransformState>().OriginalParent;
            target.SetParent(null);
        }
    }
    public void Move(List<Vector2> linePoints)
    {
        points = linePoints;
        isActivated = true;
    }

    public void SetLiftMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    private void OnDestroy()
    {
        if(this.transform.parent)
            FindChildTransform(this.transform.parent.gameObject);
            
        foreach(var _mplayer in mplayer)
        {
            Debug.Log("mplayer: "+_mplayer.gameObject.name);
            _mplayer.SetParent(null);
            _mplayer.gameObject.GetComponent<LagCompensation>().enabled = true;
        }
    }
    public void FindChildTransform(GameObject parent)
    {
        Transform child = null;

        foreach (Transform trans in parent.transform)
        {
            if (trans.gameObject.tag == "Player")
            {
                child = trans;

                if (child != null)
                    mplayer.Add(trans);
            }
            else
            {
                if (trans != null)
                    FindChildTransform(trans.gameObject);
            }
        }

        //return child;
    }
}
