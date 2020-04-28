using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{

    Hand grabbedBy = null;
    Transform grabbedLocation;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        if (grabbedBy != null)
        {
            Vector3 difference = rb.position - grabbedLocation.position;
            rb.velocity = -difference / Time.fixedDeltaTime;

            Quaternion rotationDifference = rb.rotation * Quaternion.Inverse(grabbedLocation.rotation);
            float angle;
            Vector3 axis;
            rotationDifference.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = -Mathf.Deg2Rad * angle / Time.fixedDeltaTime * axis;
            rb.angularVelocity = angularVelocity;

        }

        //Attempting to remove the force tethers from the bobber on hand squeeze
        /*
        if (rb.name == "fishing_pole")
        {
            GameObject bobberChild = rb.transform.GetChild(0).gameObject;
            Debug.Log("Child name: " + bobberChild);
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand) > .05f)
            {
                JointDrive jd = bobberChild.GetComponent<ConfigurableJoint>().xDrive;
                jd.positionSpring = 0;
                bobberChild.GetComponent<ConfigurableJoint>().xDrive = jd;
                bobberChild.GetComponent<ConfigurableJoint>().yDrive = jd;
                bobberChild.GetComponent<ConfigurableJoint>().zDrive = jd;
            }
        }
        */

    }

    public bool startGrab(Hand g, Transform grabLocation)
    {

        if (grabbedBy != null || g == null)
        {

            return false;

        }
        else
        {

            grabbedBy = g;
            grabbedLocation = grabLocation;
            return true;

        }

    }

    public bool endGrab(Hand g)
    {

        if (grabbedBy == null || g == null)
        {
            return false;
        }
        grabbedBy = null;
        grabbedLocation = null;
        return true;

    }

}
