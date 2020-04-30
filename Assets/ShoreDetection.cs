using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoreDetection : MonoBehaviour
{
    private GameObject bobber = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //This basically has the collision of the fish look for the bobber or the catch border
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bobber")
        {
            bobber = other.gameObject;
        }
        else if (other.gameObject.name == "border" && bobber != null)
        { 
            bobber.GetComponent<BobberCatch>().FishCaught();
        }

    }
}
