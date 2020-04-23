using Photon.Pun;
using UnityEngine;

public class Fish : MonoBehaviourPunCallbacks, IPunObservable
{

    public Transform fish;

    public Vector3 networkFishPos;
    public Quaternion networkFishRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fish.position);
            stream.SendNext(fish.rotation);
        }
        else
        {
            networkFishPos = (Vector3)stream.ReceiveNext();
            networkFishRot = (Quaternion)stream.ReceiveNext();
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
            fish.position = Vector3.Lerp(fish.position, networkFishPos, .05f);
            fish.rotation = Quaternion.Slerp(fish.rotation, networkFishRot, .05f);
        }
    }
}
