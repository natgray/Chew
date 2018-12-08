using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiBunny : MonoBehaviour
{

    public NavMeshAgent agent;
    private Transform chewable;
    float distance;
    public GameObject[] arr;
    float timeLeft = 5;
    public int objNumber;

    //public GameObject[] chewables;
    // Use this for initialization
    void Start()
    {
        arr = GameObject.FindGameObjectsWithTag("chewable");
        
     

        agent = GetComponent<NavMeshAgent>();

        objNumber = UnityEngine.Random.Range(0, arr.Length);

        chewable = arr[objNumber].transform;      

        if (chewable == null)
        {
            chewable = GameObject.FindWithTag("chewable").transform;
        }

        //chewables = GameObject.FindWithTag("AIBunny").transform;
        agent.SetDestination(chewable.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
		arr = GameObject.FindGameObjectsWithTag("chewable");
        distance = Vector3.Distance(chewable.transform.position, transform.position);
        if (distance < 5)
        {
            //animate.SetTrigger("attack");
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
				chewable = arr[Random.Range(0, arr.Length)].transform;
                agent.SetDestination(chewable.transform.position);
                timeLeft = 5;
            }
        }
        if (distance > 10)
        {
            //animate.SetTrigger("walk");
        }
        agent.SetDestination(chewable.transform.position);
    }
}
