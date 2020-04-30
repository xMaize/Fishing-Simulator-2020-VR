﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BobberCatch : MonoBehaviour
{
    private GameObject fish = null;
    private Vector3 startPoint;
    private bool destroyCall = false;
    private bool hitEdge = false;

    public float resistance = 5.0f;
    //public GameObject detector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fish != null)
        {
            //Keeping the box collider enabled until I can think of a fix for the fish going through the world
            //Becomes a problem when fish density is high

            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

            fish.GetComponent<NavMeshAgent>().enabled = false;
            fish.GetComponent<FishMove>().enabled = false;


            Rigidbody rb = GetComponent<Rigidbody>();
            fish.transform.position = new Vector3(transform.position.x,transform.position.y +.3f,transform.position.z);
            
            //TODO this doesnt work how it should
            //fish.transform.LookAt();

            

            rb.AddForce((startPoint - transform.position).normalized * resistance);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collider Name: " + other.gameObject.name);
        if (other.gameObject.tag == "Fish")
        {
            fish = other.gameObject;
            startPoint = other.transform.position;
        }
        else if (other.gameObject.name == "border")
        {
            hitEdge = true;
        }
    }

    public void FishCaught()
    {
        destroyCall = true;

        Debug.Log("Fish is here");
        PhotonNetwork.Destroy(fish.gameObject);

        fish = null;

        //Destroy(GetComponent<GameObject>());
    }
    /*
    public void HitWall()
    {
        Destroy(GetComponent<GameObject>());
    }
    */
    public bool DestroyBobber()
    {
        return destroyCall;
    }

    public bool HitEdge()
    {
        return hitEdge;
    }
}
