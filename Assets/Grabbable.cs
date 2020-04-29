using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{

    Hand grabbedBy = null;
    Transform offsetLocation = null; // where the object goes without use of force
    Rigidbody rb;
    Vector3 localGrabbedLocation;
    public bool useForce = false;

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
            if (!useForce)
            {
                Vector3 difference = rb.position - offsetLocation.position;
                rb.velocity = -difference / Time.fixedDeltaTime;

                Quaternion rotationDifference = rb.rotation * Quaternion.Inverse(offsetLocation.rotation);
                float angle;
                Vector3 axis;
                rotationDifference.ToAngleAxis(out angle, out axis);
                Vector3 angularVelocity = -Mathf.Deg2Rad * angle / Time.fixedDeltaTime * axis;
                rb.angularVelocity = angularVelocity;
            }
            else
            {
                Vector3 worldGrabPosition = transform.TransformPoint(localGrabbedLocation);
                Vector3 force = grabbedBy.transform.position - worldGrabPosition;
                rb.AddForceAtPosition(force, worldGrabPosition);
                Debug.DrawRay(worldGrabPosition, force);
            }
        }
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
            offsetLocation = grabLocation;
            localGrabbedLocation = transform.InverseTransformPoint(g.transform.position);
            Debug.Log(localGrabbedLocation);
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
        offsetLocation = null;
        return true;

    }

}
