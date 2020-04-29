using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatScript : MonoBehaviour
{
    public GameObject water;
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
        if (transform.position.y > water.transform.position.y)
        {
            //rb.useGravity = true;
            rb.drag = 1;
        }
        else
        {
            //rb.useGravity = false;
            rb.AddForce(-1 * Physics.gravity + new Vector3(0, 0.1f, 0));
            rb.drag = 10;
        }
    }
}
