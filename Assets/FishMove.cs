﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FishMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(randomHit());
    }

    private void Update()
    {

    }

    //Gives the fish a new destination on the NavMesh every 10 seconds
    IEnumerator randomHit()
    {
        bool firstPass = true;
        Vector3 prevDirection = new Vector3(0, 0, 0);

        while (true)
        {

            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            Vector3 randomDirection = Random.insideUnitSphere * 15;

            if (!firstPass)
            {
                if (((randomDirection.x > 0f && prevDirection.x > 0f) && (randomDirection.z > 0f && prevDirection.z > 0f)) ||
                  ((randomDirection.x < 0f && prevDirection.x < 0f) && (randomDirection.z < 0f && prevDirection.z < 0f)) ||
                  ((randomDirection.x > 0f && prevDirection.x > 0f) && (randomDirection.z < 0f && prevDirection.z < 0f)) ||
                  ((randomDirection.x < 0f && prevDirection.x < 0f) && (randomDirection.z > 0f && prevDirection.z > 0f)))
                {
                    randomDirection = Random.insideUnitSphere * 15;
                }
            }

            prevDirection = randomDirection;
            firstPass = false;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 15, 1);
            Vector3 finalPosition = hit.position;

            if (agent.isActiveAndEnabled) {
                agent.destination = finalPosition;
            }

            yield return new WaitForSeconds(8);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4f);
            int i = 0;

            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == "Bobber")
                {
                    agent.destination = hitColliders[i].attachedRigidbody.position;
                    yield return new WaitForSeconds(5);
                    //Debug.Log("Is Caught: " + isCaught);
                }
                i++;
            }
        }
    }
}
