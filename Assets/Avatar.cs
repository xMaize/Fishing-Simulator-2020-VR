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
    //public PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rig.head.position);
            stream.SendNext(rig.head.rotation); 
            stream.SendNext(rig.leftHand.position);
            stream.SendNext(rig.rightHand.position);
        }
        else
        {
            head.position = (Vector3)stream.ReceiveNext();
            head.rotation = (Quaternion)stream.ReceiveNext();
            leftHand.position = (Vector3)stream.ReceiveNext();
            rightHand.position = (Vector3)stream.ReceiveNext();
        }
    }

    void Start()
    {
        //head.gameObject.SetActive(false);
        //body.gameObject.SetActive(false);
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
    }
}
