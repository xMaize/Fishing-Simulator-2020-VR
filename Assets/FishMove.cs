using System.Collections;
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

    // Update is called once per frame
    void Update()
    {

    }
    //Gives the fish a new destination on the NavMesh every 10 seconds
    IEnumerator randomHit()
    {
        while (true)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            Vector3 randomDirection = Random.insideUnitSphere * 15;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 15, 1);
            Vector3 finalPosition = hit.position;

            agent.destination = finalPosition;

            yield return new WaitForSeconds(3);
        }

    }
}
