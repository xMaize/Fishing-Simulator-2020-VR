using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGearPhoton : MonoBehaviourPunCallbacks, IPunObservable
{
    //public Transform rod;
    //public Transform bobber;
    //public Transform crank;

    public GameObject rod;
    public GameObject bobber;
    public GameObject crank;

    public Vector3 networkRodPos;
    public Quaternion networkRodRot;
    public Vector3 networkBobberPos;
    public Quaternion networkBobberRot;
    public Vector3 networkCrankPos;
    public Quaternion networkCrankRot;
    
    public Vector3 networkRodRbPos;
    public Quaternion networkRodRbRot;
    public Vector3 networkBobberRbPos;
    public Quaternion networkBobberRbRot;
    public Vector3 networkCrankRbPos;
    public Quaternion networkCrankRbRot;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rod.transform.position);
            stream.SendNext(rod.transform.rotation);
            stream.SendNext(bobber.transform.position);
            stream.SendNext(bobber.transform.rotation);
            stream.SendNext(crank.transform.position);
            stream.SendNext(crank.transform.rotation);
            /*
            stream.SendNext(rod.GetComponent<Rigidbody>().transform.position);
            stream.SendNext(rod.GetComponent<Rigidbody>().transform.rotation);
            stream.SendNext(bobber.GetComponent<Rigidbody>().transform.position);
            stream.SendNext(bobber.GetComponent<Rigidbody>().transform.rotation);
            stream.SendNext(crank.GetComponent<Rigidbody>().transform.position);
            stream.SendNext(crank.GetComponent<Rigidbody>().transform.rotation);
            */

        }
        else
        {
            networkRodPos = (Vector3)stream.ReceiveNext();
            networkRodRot = (Quaternion)stream.ReceiveNext();
            networkBobberPos = (Vector3)stream.ReceiveNext();
            networkBobberRot = (Quaternion)stream.ReceiveNext();
            networkCrankPos = (Vector3)stream.ReceiveNext();
            networkCrankRot = (Quaternion)stream.ReceiveNext();

            /*
            networkRodRbPos = (Vector3)stream.ReceiveNext();
            networkRodRbRot = (Quaternion)stream.ReceiveNext();
            networkBobberRbPos = (Vector3)stream.ReceiveNext();
            networkBobberRbRot = (Quaternion)stream.ReceiveNext();
            networkCrankRbPos = (Vector3)stream.ReceiveNext();
            networkCrankRbRot = (Quaternion)stream.ReceiveNext();
            */
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
            rod.transform.position = Vector3.Lerp(rod.transform.position, networkRodPos, .05f);
            rod.transform.rotation = Quaternion.Slerp(rod.transform.rotation, networkRodRot, .05f);

            bobber.transform.position = Vector3.Lerp(bobber.transform.position, networkBobberPos, .05f);
            bobber.transform.rotation = Quaternion.Slerp(bobber.transform.rotation, networkBobberRot, .05f);

            crank.transform.position = Vector3.Lerp(crank.transform.position, networkCrankPos, .05f);
            crank.transform.rotation = Quaternion.Slerp(crank.transform.rotation, networkCrankRot, .05f);
            /*
            rod.GetComponent<Rigidbody>().transform.position = Vector3.Lerp(rod.GetComponent<Rigidbody>().transform.position, networkRodRbPos, .05f);
            rod.GetComponent<Rigidbody>().transform.rotation = Quaternion.Slerp(rod.GetComponent<Rigidbody>().transform.rotation, networkRodRbRot, .05f);

            bobber.GetComponent<Rigidbody>().transform.position = Vector3.Lerp(bobber.GetComponent<Rigidbody>().transform.position, networkBobberRbPos, .05f);
            bobber.GetComponent<Rigidbody>().transform.rotation = Quaternion.Slerp(bobber.GetComponent<Rigidbody>().transform.rotation, networkBobberRbRot, .05f);

            crank.GetComponent<Rigidbody>().transform.position = Vector3.Lerp(crank.GetComponent<Rigidbody>().transform.position, networkCrankRbPos, .05f);
            crank.GetComponent<Rigidbody>().transform.rotation = Quaternion.Slerp(crank.GetComponent<Rigidbody>().transform.rotation, networkCrankRbRot, .05f);
            */
        }
        
    }
}
