using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float moveSpeed = 5.0f;

    bool isActivated = false;
    List<Vector2> points;
    int nowPoint = 0;

    void Update()
    {
        if(isActivated)
        {
            //Debug.Log(nowPoint);

            // move distance
            Vector2 dist = 0.5f * (points[nowPoint + 1] - points[nowPoint]).normalized * moveSpeed * Time.deltaTime;
            // judge
            Vector2 nowPos = new Vector2(transform.position.x, transform.position.y);
            if(dist.magnitude <= (points[nowPoint + 1] - nowPos).magnitude) {
                transform.position += new Vector3(dist.x, dist.y, 0);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.left, dist));
            }
            else {
                nowPoint += 1;
                if(nowPoint == points.Count - 1) nowPoint = 0;
                transform.position = new Vector3(points[nowPoint].x, points[nowPoint].y, 0);
                Vector2 tmp = points[nowPoint + 1] - points[nowPoint];
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.left, tmp));
            }
        }
    }

    public void Move(List<Vector2> linePoints)
    {
        points = linePoints;
        isActivated = true;
    }

    public void SetArrowMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
