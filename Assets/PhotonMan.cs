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

    private List<Vector3> spawns = new List<Vector3>();
    private bool isConnected = false;


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
        isConnected = true;
        //OVRCameraRig camera = PhotonNetwork.Instantiate(cameraPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<OVRCameraRig>();

        //We need to instantiate the fish in this script to get them to destroy in the scene properly
        //For now all there is is this one spot so we should add like 2 more
        

        Avatar avatar = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Avatar>();
        avatar.rig = rig;

        FishGearPhoton fishGear = PhotonNetwork.Instantiate(fishGearPrefab.name, fishGearSpawn.position, fishGearSpawn.rotation).GetComponent<FishGearPhoton>();

    }
    // Update is called once per frame
    void Update()
    {
        spawns.Add(new Vector3(0, 0.5f, 14));
        spawns.Add(new Vector3(15, 0.5f, 5));
        spawns.Add(new Vector3(6, 0.5f, -18));
        spawns.Add(new Vector3(-8, 0.5f, -15));
        spawns.Add(new Vector3(-18, 0.5f, 4));

        if (isConnected)
        {
            if(GameObject.FindGameObjectsWithTag("Fish").Length < 15)
            {
                Fish fish = PhotonNetwork.Instantiate(fishPrefab.name, spawns[Random.Range(0,6)], Quaternion.identity).GetComponent<Fish>();
            }
        }
    }
}
