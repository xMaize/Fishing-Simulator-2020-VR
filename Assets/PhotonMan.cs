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
    public GameObject fishGearPrefab;
    public GameObject fishPrefab;
    public GameObject water;
    public Transform fishGearSpawn;
    //List<Fish> schoolOfFish = new List<Fish>();
    //Vector3[] fishSpawns = new Vector3[10];
    public Rig rig;

    void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(Reconnect());
    }

    IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(5f);
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
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
        //OVRCameraRig camera = PhotonNetwork.Instantiate(cameraPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<OVRCameraRig>();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                Fish fish = PhotonNetwork.Instantiate(fishPrefab.name, new Vector3(0, 0.5f, 20), Quaternion.identity).GetComponent<Fish>();
            }
        }

        Avatar avatar = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Avatar>();
        avatar.rig = rig;

        FishGearPhoton fishGear = PhotonNetwork.Instantiate(fishGearPrefab.name, fishGearSpawn.position, fishGearSpawn.rotation).GetComponent<FishGearPhoton>();
        FloatScript bobber = fishGear.bobber.GetComponent<FloatScript>();
        bobber.water = water;
        FloatScript rod = fishGear.rod.GetComponent<FloatScript>();
        rod.water = water;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
