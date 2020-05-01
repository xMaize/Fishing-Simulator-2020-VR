using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{

    Hand grabbedBy = null;
    Rigidbody rb;
    Vector3 localGrabbedLocation;
    Transform offsetLocation = null;
    public bool useForce = false;
    public bool useExact = false;
    public bool isSpinning;

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
            if (!useForce && !useExact)
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
            else if (useForce && !useExact)
            {
                isSpinning = true;
                Vector3 worldGrabPosition = transform.TransformPoint(localGrabbedLocation);
                Vector3 force = grabbedBy.transform.position - worldGrabPosition;
                rb.AddForceAtPosition(3 * force, worldGrabPosition);
            }
            else
            {
                HingeJoint hj = GetComponent<HingeJoint>();
                if (hj != null)
                {
                    Vector3 worldGrabPosition = transform.TransformPoint(localGrabbedLocation);
                    Vector3 A = worldGrabPosition - hj.connectedAnchor;
                    Vector3 B = grabbedBy.transform.position - hj.connectedAnchor;
                    Vector3 axis = hj.axis;
                    float dotAaxis = Vector3.Dot(A, axis);
                    float dotBaxis = Vector3.Dot(B, axis);
                    A = A - dotAaxis * axis;
                    B = B - dotBaxis * axis;
                    float angle = Vector3.SignedAngle(A.normalized, B.normalized, axis);
                    transform.Rotate(hj.axis, angle, Space.Self);
                }
            }
        }
        else
        {
            if(useForce && rb.velocity.magnitude < 0.2f)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
                isSpinning = false;
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
