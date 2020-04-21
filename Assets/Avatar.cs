using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviourPunCallbacks, IPunObservable
{
    // Start is called before the first frame update

    public Rig rig;
    public Transform head;
    public Transform body;
    public Transform leftHand;
    public Transform rightHand;

    public Vector3 networkHeadPos;
    public Quaternion networkHeadRot;
    public Vector3 networkBodyPos;
    public Quaternion networkBodyRot;
    public Vector3 networkLeftHandPos;
    public Quaternion networkLeftHandRot;
    public Vector3 networkRightHandPos;
    public Quaternion networkRightHandRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rig.head.position);
            stream.SendNext(rig.head.rotation);
            stream.SendNext(new Vector3(rig.head.position.x, rig.head.position.y - .5f, rig.head.position.z));
            stream.SendNext(new Quaternion(0, rig.head.rotation.y, 0, rig.head.rotation.w));
            stream.SendNext(rig.leftHand.position);
            stream.SendNext(rig.leftHand.rotation);
            stream.SendNext(rig.rightHand.position);
            stream.SendNext(rig.rightHand.rotation);
        }
        else
        {
            networkHeadPos = (Vector3)stream.ReceiveNext();
            networkHeadRot = (Quaternion)stream.ReceiveNext();
            networkBodyPos = (Vector3)stream.ReceiveNext();
            networkBodyRot = (Quaternion)stream.ReceiveNext();
            networkLeftHandPos = (Vector3)stream.ReceiveNext();
            networkLeftHandRot = (Quaternion)stream.ReceiveNext();
            networkRightHandPos = (Vector3)stream.ReceiveNext();
            networkRightHandRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void Start()
    {
        if (photonView.IsMine) {
            head.gameObject.SetActive(false);
            body.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            head.position = rig.head.position;
            body.position = new Vector3(head.position.x, head.position.y - .5f, head.position.z);
            leftHand.position = rig.leftHand.position;
            rightHand.position = rig.rightHand.position;

            head.rotation = rig.head.rotation;
            body.rotation = new Quaternion(0, head.rotation.y, 0, head.rotation.w);
            leftHand.rotation = rig.leftHand.rotation;
            rightHand.rotation = rig.rightHand.rotation;
        }
        else
        {
            head.position = Vector3.Lerp(head.position, networkHeadPos, .05f);
            head.rotation = Quaternion.Slerp(head.rotation, networkHeadRot, .05f);
            leftHand.position = Vector3.Lerp(leftHand.position, networkLeftHandPos, .05f); 
            leftHand.rotation = Quaternion.Slerp(leftHand.rotation, networkLeftHandRot, .05f);
            body.position = Vector3.Lerp(body.position, networkBodyPos, .05f);
            body.rotation = Quaternion.Slerp(body.rotation, networkBodyRot, .05f);
            rightHand.position = Vector3.Lerp(rightHand.position, networkRightHandPos, .05f);
            rightHand.rotation = Quaternion.Slerp(rightHand.rotation, networkRightHandRot, .05f);
        }
        
        if (rig.right.isHolding())
        {
            rightHand.gameObject.SetActive(false);
        }
        else
        {
            rightHand.gameObject.SetActive(true);
        }

        if (rig.left.isHolding())
        {
            leftHand.gameObject.SetActive(false);
        }
        else
        {
            leftHand.gameObject.SetActive(true);
        }

    }

}
