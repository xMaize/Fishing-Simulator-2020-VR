using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    public float reelStrength = 1000f;
    public GameObject physicsBobber;
    public AudioClip whoosh;
    bool isVibrating = false;

    //for the rod
    bool isCast = false;
    bool hitWall = false;
    bool hitIsland = false;
    bool destroyBobber = false;
    Vector3 prevPos1 = new Vector3();
    Vector3 prevPos2 = new Vector3();
    GameObject rbBobber = null;


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

        if (!isHolding())
        {
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
        else if (grabbedObject.name == "fishing_pole")
        {
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            GameObject bobberChild = rb.transform.GetChild(0).gameObject;
            Grabbable crank = rb.transform.parent.transform.GetChild(1).gameObject.GetComponent<Grabbable>();

            if (Time.frameCount % 4 == 0)
                prevPos1 = bobberChild.transform.position;

            if (Time.frameCount % 2 == 0 && Time.frameCount % 4 != 0)
                prevPos2 = bobberChild.transform.position;


            Vector3 throwAngle = (prevPos2 + prevPos1) / 2f;
            //Creates new physics bobber in place of old
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myHand) > .20f && isCast == false)
            {
                rbBobber = PhotonNetwork.Instantiate(physicsBobber.name, bobberChild.transform.position, bobberChild.transform.rotation);

                //Gives ownership to whoever created it
                PhotonView photonView = rbBobber.GetComponent<PhotonView>();
                if (photonView != null)
                {
                    if (!photonView.IsMine)
                    {
                        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                    }
                }

                Vector3 throwVector = bobberChild.transform.position - throwAngle;
                bobberChild.GetComponent<MeshRenderer>().enabled = false;
                Debug.Log("Throw Vector: " + throwVector.ToString());

                AudioSource.PlayClipAtPoint(whoosh, grabbedObject.transform.position);
                rbBobber.GetComponent<Rigidbody>().AddForce(throwVector * 25, ForceMode.Impulse);
                isCast = true;

            }
            //temporary method of returning the bobber to the rod
            //TODO: implement working crank mechanic if possible
            else if (crank.isSpinning && rbBobber != null)
            {
                rbBobber.GetComponent<Rigidbody>().AddForce((grabbedObject.transform.position - rbBobber.transform.position).normalized * reelStrength);
                destroyBobber = rbBobber.GetComponent<BobberCatch>().DestroyBobber();
                hitWall = rbBobber.GetComponent<BobberCatch>().HitEdge();
                hitIsland = rbBobber.GetComponent<BobberCatch>().HitIsland();

                if (hitWall || destroyBobber || hitIsland)
                {
                    bobberChild.GetComponent<MeshRenderer>().enabled = true;
                    PhotonNetwork.Destroy(rbBobber);
                    isCast = false;
                    rbBobber = null;
                }
            }


            if (rbBobber != null)
            {
                if (rbBobber.GetComponent<BobberCatch>().IsHooked())
                {
                    StartCoroutine(vibrateController());
                }
            }
        }

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

        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);

        if (triggerValue < 0.2f && grabbedObject != null)
        {
            if (grabbedObject.endGrab(this))
            {
                grabbedObject = null;
            }
        }

    }

    private void FixedUpdate()
    {

    }
    
    private void OnTriggerStay(Collider other)
    {
        

        Rigidbody otherRb = other.attachedRigidbody;
        if (otherRb == null)
        {
            return;
        }

        PhotonView photonView = otherRb.GetComponent<PhotonView>();
        if (photonView != null)
        {
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
        if (otherRb.transform.childCount > 0)
        {
            photonView = otherRb.transform.GetChild(0).GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }
            }
        }
        Grabbable gr = otherRb.GetComponent<Grabbable>();
        if (gr == null)
        {
            return;
        }

        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);
        Debug.Log(this.name + ": " + triggerValue);

        if(triggerValue > 0.3f && grabbedObject == null)
        {
            attachPoint.position = otherRb.position;
            attachPoint.rotation = otherRb.rotation;
            if (gr.startGrab(this, attachPoint))
            {
                grabbedObject = gr;
            }
        }
        else if (triggerValue < 0.2f && grabbedObject != null)
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

    IEnumerator vibrateController()
    {
        if (!isVibrating) {
            isVibrating = true;
            VibrationManager.manager.TriggerVibration(255, 2, 255);
            yield return new WaitForSeconds(1);
            isVibrating = false;
        }
        else
        {
            yield return null;
        }
    }

}
