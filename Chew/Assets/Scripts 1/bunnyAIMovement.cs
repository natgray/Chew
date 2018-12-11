using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bunnyAIMovement : MonoBehaviour {

    public NavMeshAgent agent;
    private Transform chewable;
    float distance;

    //public GameObject[] chewables;
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();

        if (chewable == null)
        {
            chewable = GameObject.FindWithTag("chewable").transform;
        }

        //chewables = GameObject.FindWithTag("AIBunny").transform;
        

        agent.SetDestination(chewable.transform.position);

    }
	
	// Update is called once per frame
	void Update () {
        //distance = Vector3.Distance(Player.position, transform.position);
        if (distance < 10)
        {
            //animate.SetTrigger("attack");
        }
        if (distance > 10)
        {
            //animate.SetTrigger("walk");
        }
        //agent.SetDestination(Player.position);
    }
}
