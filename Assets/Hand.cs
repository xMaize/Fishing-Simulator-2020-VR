using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    Rigidbody attachedRigidBody;
    public OVRInput.Controller myHand;
    public Transform attachPoint;

    // Start is called before the first frame update
    void Start()
    {
        attachPoint = Instantiate<GameObject>(new GameObject(), this.transform).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (attachedRigidBody != null)
        {
            Vector3 difference = attachedRigidBody.position - attachPoint.position;
            attachedRigidBody.velocity = -difference / Time.fixedDeltaTime;

            Quaternion rotationDifference = attachedRigidBody.rotation * Quaternion.Inverse(attachPoint.rotation);
            float angle;
            Vector3 axis;
            rotationDifference.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = -Mathf.Deg2Rad * angle / Time.fixedDeltaTime * axis;
            attachedRigidBody.angularVelocity = angularVelocity;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;
        
        if (otherRb == null)
        {
            return;
        }

        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myHand);

        if(triggerValue > 0.05f && attachedRigidBody == null)
        {
            attachedRigidBody = otherRb;
            attachedRigidBody.maxAngularVelocity = Mathf.Infinity;
            attachPoint.position = attachedRigidBody.position;
            attachPoint.rotation = attachedRigidBody.rotation;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (triggerValue < 0.04f && attachedRigidBody != null)
        {
            attachedRigidBody = null;
            this.GetComponent<MeshRenderer>().enabled = true;
        }        
     
    }
}
