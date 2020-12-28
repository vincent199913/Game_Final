using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    float moveSpeed = 5.0f;
    float rotationSpeed = 20.0f;
    float nowDegree = 0.0f;

    bool isActivated = false;
    List<Vector2> points;
    int nowPoint = 0;
    int direct = 1;

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

            // rotate (drop when use animation)
            transform.rotation = Quaternion.Euler(Vector3.forward * nowDegree);
            nowDegree += direct * rotationSpeed;
        }
    }

    public void Move(List<Vector2> linePoints)
    {
        points = linePoints;
        isActivated = true;
    }

    public void SetGearMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetGearRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
