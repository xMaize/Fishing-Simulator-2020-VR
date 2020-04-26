using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGearPhoton : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform rod;
    public Transform bobber;
    public Transform crank;

    public Vector3 networkRodPos;
    public Quaternion networkRodRot;
    public Vector3 networkBobberPos;
    public Quaternion networkBobberRot;
    public Vector3 networkCrankPos;
    public Quaternion networkCrankRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rod.position);
            stream.SendNext(rod.rotation);
            stream.SendNext(bobber.position);
            stream.SendNext(bobber.rotation);
            stream.SendNext(crank.position);
            stream.SendNext(crank.rotation);

        }
        else
        {
            networkRodPos = (Vector3)stream.ReceiveNext();
            networkRodRot = (Quaternion)stream.ReceiveNext();
            networkBobberPos = (Vector3)stream.ReceiveNext();
            networkBobberRot = (Quaternion)stream.ReceiveNext();
            networkCrankPos = (Vector3)stream.ReceiveNext();
            networkCrankRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!photonView.IsMine)
        {
            rod.position = Vector3.Lerp(rod.position, networkRodPos, .05f);
            rod.rotation = Quaternion.Slerp(rod.rotation, networkRodRot, .05f);

            bobber.position = Vector3.Lerp(bobber.position, networkBobberPos, .05f);
            bobber.rotation = Quaternion.Slerp(bobber.rotation, networkBobberRot, .05f);

            crank.position = Vector3.Lerp(crank.position, networkCrankPos, .05f);
            crank.rotation = Quaternion.Slerp(crank.rotation, networkCrankRot, .05f);
        }
        
    }
}
