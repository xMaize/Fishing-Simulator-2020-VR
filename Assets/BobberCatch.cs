using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BobberCatch : MonoBehaviour
{
    private GameObject fish = null;
    private Vector3 startPoint;
    private bool isCast = false;

    public float resistance = 2.0f;
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
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            fish.GetComponent<NavMeshAgent>().enabled = false;
            fish.GetComponent<FishMove>().enabled = false;


            Rigidbody rb = GetComponent<Rigidbody>();
            fish.transform.position = transform.position;
            //TODO this doesnt work how it should
            fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, Quaternion.LookRotation(transform.parent.position).normalized, Time.deltaTime * 5);

            

            rb.AddForce((startPoint - transform.position).normalized * resistance);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider Name: " + other.gameObject.name);
        if(other.gameObject.name == "fish1(Clone)" || other.gameObject.name == "fish1")
        {
            fish = other.gameObject;
            startPoint = other.transform.position;
            isCast = true;
        }
    }

    public void BorderDetected()
    {
        JointDrive jd = GetComponent<ConfigurableJoint>().xDrive;
        jd.positionSpring = 150;
        GetComponent<ConfigurableJoint>().xDrive = jd;
        GetComponent<ConfigurableJoint>().yDrive = jd;
        GetComponent<ConfigurableJoint>().zDrive = jd;

        isCast = false;

        Destroy(fish);

        fish = null;

        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public bool IsCast()
    {
        return isCast;
    }
}
