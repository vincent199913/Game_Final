﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LagCompensation : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector2 verlocity;
    private float angularVelocity;
    private bool valuesReceived;
    private float currentTime;
    private double currentPacket;
    private double lastPacket;
    private Vector3 positionLastPacket = Vector3.zero;
    private Quaternion roatationLastPacket = Quaternion.identity;
    private Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rig.velocity);
            stream.SendNext(rig.angularVelocity);
            return;
        }
        lastPos = (Vector3)stream.ReceiveNext();
        lastRot = (Quaternion)stream.ReceiveNext();
        verlocity = (Vector2)stream.ReceiveNext();
        angularVelocity = (float)stream.ReceiveNext();
        valuesReceived = true;
        currentTime = 0f;
        lastPacket = currentPacket;
        currentPacket = info.SentServerTime;
        positionLastPacket = transform.position;
        roatationLastPacket = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && valuesReceived)
        {
            double num = currentPacket - lastPacket;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionLastPacket, lastPos, (float)(currentTime / num));
            transform.rotation = Quaternion.Lerp(roatationLastPacket, lastRot, (float)(currentTime / num));
            rig.velocity = verlocity;
            rig.angularVelocity = angularVelocity;
        }
    }
}