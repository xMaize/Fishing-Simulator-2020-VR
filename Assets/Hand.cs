using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    Rigidbody attachedRigidBody;
    public OVRInput.Controller myHand;
    public Transform attachPoint;
    public Transform Laser;
    public Transform trackingSpace;
    public float speed;
    public Transform head;
    bool isGrabbing = false;

    // Start is called before the first frame update
    void Start()
    {
        attachPoint = Instantiate<GameObject>(new GameObject(), this.transform).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(Laser.position, Laser.forward);
        RaycastHit[] hits = Physics.RaycastAll(r, 10.0f);
        
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        Laser.localScale = new Vector3(0, 0, 0);


            for (int i = 0; i < hits.Length; i++)
            {
                Laser.localScale = new Vector3(1, 1, hits[i].distance);
                RaycastHit hit = hits[i];
                Rigidbody rb = hit.rigidbody;
                if (rb != null)
                {
                    float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myHand);

                    if (triggerValue > 0.05f && attachedRigidBody == null)
                    {
                        attachedRigidBody = rb;
                        attachPoint.position = this.transform.position;
                        attachPoint.rotation = this.transform.rotation;
                    }
                    break;
                }
                else
                {
                    if (attachedRigidBody == null)
                    {
                        //teleportation
                        Vector3 targetPoint = hit.point;
                        Vector3 footPos = head.position;
                        footPos.y -= head.localPosition.y;
                        Vector3 offset = targetPoint - footPos;

                        bool trigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, myHand);

                        if (trigger)
                        {
                            trackingSpace.Translate(offset, Space.World);
                        }
                        break;
                    }//if
                }//else   
            }//for

        Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, myHand);
        float up = joystickInput.y;
        float right = joystickInput.x;

        Vector3 headForwardVector = head.forward;
        headForwardVector.y = 0;
        headForwardVector.Normalize();
        Vector3 headRightVector = head.right;
        headRightVector.y = 0;
        headRightVector.Normalize();

        Vector3 direction = headForwardVector * up + headRightVector * right;

        trackingSpace.transform.Translate(direction * speed * Time.deltaTime, Space.World);

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
        }
        else if (triggerValue < 0.04f && attachedRigidBody != null)
        {
            attachedRigidBody = null;
        }        
     
    }

    public bool isHolding()
    {
        return attachedRigidBody != null;
    }
    
}
