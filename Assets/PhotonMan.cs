using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMan : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public enum MessageTypes
    {

    }

    public GameObject playerPrefab;
    public Rig rig;
    void Start()
    {
        PhotonNetwork.SendRate = 2;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to master");
        PhotonNetwork.JoinOrCreateRoom("room", new Photon.Realtime.RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        Debug.Log("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Avatar avatar = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Avatar>();
        avatar.rig = rig;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
