using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public Transform handAnchor;
    Grabbable grabbedObject;
    public OVRInput.Controller myHand;
    public Transform attachPoint;
    public Transform Laser;
    public Transform trackingSpace;
    public float speed;
    public Transform head;
    public float snapRotateDelta = 25;
    bool canSnapRotate = true;
    public GameObject arcPointPrefab;
    public float arcSpeed;
    List<GameObject> arcPoints = new List<GameObject>();
    public bool useGoGo;
    bool fishingRodIsHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        attachPoint = Instantiate<GameObject>(new GameObject(), this.transform).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 footPos = head.position;
        footPos.y -= head.localPosition.y;

        Vector3 headToController = handAnchor.position - head.position;
        float d = headToController.magnitude;
        float s = 1;

        if (d > .5f)
        {
            s = 1 + 10 * (d - .5f);
        }

        transform.position = head.position + s * d * headToController;

        //generate an arc, eminating from the controller laser forward and going outward at a certain velocity
        Vector3 arcVelocity = Laser.forward * arcSpeed;
        Vector3 arcPos = Laser.position;
        float distance = 0;

        foreach (GameObject p in arcPoints)
        {
            GameObject.Destroy(p);
        }

        arcPoints.Clear();

        if (!isHolding()) {
            while (distance < 10)
            {
                Vector3 delta_p = arcVelocity * .01f;

                //perform a raycast to determine if our arc hit anything
                RaycastHit[] hits = Physics.RaycastAll(arcPos, arcVelocity.normalized, delta_p.magnitude);
                bool arcHit = false;
                Vector3 arcHitPoint = Vector3.zero;

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    GameObject go2 = GameObject.Instantiate(arcPointPrefab, arcPos, Quaternion.identity);
                    go2.transform.forward = arcVelocity.normalized;
                    go2.transform.localScale = new Vector3(1, 1, .01f);
                    arcPoints.Add(go2);
                    arcHit = true;
                    arcHitPoint = hit.point;
                    break;
                }

                if (arcHit)
                {
                    //teleportation
                    Vector3 targetPoint = arcHitPoint;
                    Vector3 offset = targetPoint - footPos;
                    bool trigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, myHand);

                    if (trigger)
                    {
                        trackingSpace.Translate(offset, Space.World);
                    }

                    break;
                }

                arcPos += delta_p;
                arcVelocity += new Vector3(0, -9.8f, 0) * .01f;
                distance += delta_p.magnitude;

                GameObject go = GameObject.Instantiate<GameObject>(arcPointPrefab, arcPos, Quaternion.identity);
                go.transform.forward = arcVelocity.normalized;
                go.transform.localScale = new Vector3(1, 1, delta_p.magnitude);
                arcPoints.Add(go);
            }
        }

        /*
        Ray r = new Ray(Laser.position, Laser.forward);
        RaycastHit[] hits = Physics.RaycastAll(r, 10.0f);
        
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        Laser.localScale = new Vector3(0, 0, 0);


        for (int i = 0; i < hits.Length; i++)
        {
            Laser.localScale = new Vector3(1, 1, hits[i].distance);
            RaycastHit hit = hits[i];
            Rigidbody rb = hit.rigidbody;
            Grabbable gr = null;
            if (rb != null)
            {
                gr = rb.GetComponent<Grabbable>();
            }
            if (gr != null)
            {
                float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myHand);

                if (triggerValue > 0.05f && grabbedObject == null)
                {
                    attachPoint.position = rb.position;
                    attachPoint.rotation = rb.rotation;
                    if (gr.startGrab(this, attachPoint))
                    {
                        grabbedObject = gr;
                    }
                }
                else if (triggerValue < 0.04f && grabbedObject != null)
                {
                    if (gr.endGrab(this))
                    {
                        grabbedObject = null;
                    }
                }
                break;
            }
            else
            {
                if (gr == null)
                {
                    //teleportation
                    Vector3 targetPoint = hit.point;
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
        */

        Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, myHand);
        float up = joystickInput.y;
        float right = joystickInput.x;

        Vector3 headForwardVector = head.forward;
        headForwardVector.y = 0;
        headForwardVector.Normalize();
        /*
        Vector3 headRightVector = head.right;
        headRightVector.y = 0;
        headRightVector.Normalize();
        */
        Vector3 direction = headForwardVector * up; //+ headRightVector * right;

        trackingSpace.transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float rightMag = Mathf.Abs(right);

        if (rightMag > .5f && canSnapRotate)
        {
            trackingSpace.transform.RotateAround(footPos, Vector3.up, snapRotateDelta * Mathf.Sign(right));
            canSnapRotate = false;
        }

        if (rightMag < .2f)
        {
            canSnapRotate = true;
        }

    }

    private void FixedUpdate()
    {
        /*
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
            
            //Attempting to remove the force tethers from the bobber on hand squeeze
            if(attachedRigidBody.name == "fishing_pole")
            {
                GameObject bobberChild = attachedRigidBody.transform.GetChild(0).gameObject;
                Debug.Log("Child name: " + bobberChild);
                if(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand) > .05f)
                {
                    JointDrive jd = bobberChild.GetComponent<ConfigurableJoint>().xDrive;
                    jd.positionSpring = 0;
                    bobberChild.GetComponent<ConfigurableJoint>().xDrive = jd;
                    bobberChild.GetComponent<ConfigurableJoint>().yDrive = jd;
                    bobberChild.GetComponent<ConfigurableJoint>().zDrive = jd;
                }
            }
            
        }
        */
    }
    
    private void OnTriggerStay(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;
        if (otherRb == null)
        {
            return;
        }
        Grabbable gr = otherRb.GetComponent<Grabbable>();
        if (gr == null)
        {
            return;
        }

        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);

        if(triggerValue > 0.05f && grabbedObject == null)
        {
            attachPoint.position = otherRb.position;
            attachPoint.rotation = otherRb.rotation;
            if (gr.startGrab(this, attachPoint))
            {
                grabbedObject = gr;
            }
        }
        else if (triggerValue < 0.04f && grabbedObject != null)
        {
            if (gr.endGrab(this))
            {
                grabbedObject = null;
            }
        }        
     
    }

    public bool isHolding()
    {
        return grabbedObject != null;
    }
    
}
