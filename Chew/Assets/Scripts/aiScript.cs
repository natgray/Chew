using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiScript : MonoBehaviour {
    private Transform Player;
    public NavMeshAgent agent;

    //Animator animate;
    float distance;
    int health = 0;
    public GameObject[] arr;
    public int objNumber;
    float timeLeft = 5;


    // Use this for initialization
    void Start () {
        //animate = GetComponent<Animator>();
        //animate.SetTrigger("walk");

        arr = GameObject.FindGameObjectsWithTag("Player");
        objNumber = UnityEngine.Random.Range(0, arr.Length);
        Player = arr[objNumber].transform;



        agent = GetComponent<NavMeshAgent>();
        //Player = GameObject.FindWithTag("Player").transform;
        agent.SetDestination(Player.position);
        //col = GameObject.FindWithTag("col");
        //Collider collider = GetChild(0).GetComponent<Collider>();


    }

    // Update is called once per frame
    void Update () {
        distance = Vector3.Distance(Player.position, transform.position);
        if (distance < 5)
        {
            
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                objNumber = UnityEngine.Random.Range(0, arr.Length);
                Player = arr[objNumber].transform;
                agent.SetDestination(Player.transform.position);
                timeLeft = 5;
            }
        }
        if (distance > 10)
        {
            //animate.SetTrigger("walk");
        }
        agent.SetDestination(Player.position);

    }
}
