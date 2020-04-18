using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Interpolation : MonoBehaviourPunCallbacks, IPunObservable
{
    public Vector3 networkPos  = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine) transform.position = Vector3.Lerp(transform.position, networkPos, .05f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            networkPos = (Vector3)stream.ReceiveNext();
        }
    }

}
