using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiScript : MonoBehaviour {
    private Transform Player;
    public Transform Player1;
    public GameObject PlayerLocation;
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
        PlayerLocation = GameObject.FindGameObjectWithTag("Player");


        agent = GetComponent<NavMeshAgent>();
        //Player = GameObject.FindWithTag("Player").transform;
        agent.SetDestination(Player.transform.position);
        //col = GameObject.FindWithTag("col");
        //Collider collider = GetChild(0).GetComponent<Collider>();
        distance = Vector3.Distance(Player.position, transform.position);

    }

    // Update is called once per frame
    void Update () {
        distance = Vector3.Distance(Player.position, transform.position);
      
        if (distance < 5)
        {
            timeLeft -= Time.deltaTime;
            Debug.Log("doing Damage");
        }
        if (timeLeft < 0)
        {
            agent.SetDestination(Player.transform.position);
            timeLeft = 5;
            Debug.Log("Picking new target");
        }

        if (timeLeft > 0 && timeLeft < 5 && PlayerLocation)
        {
            //agent.SetDestination(Player.transform.position);
            /* objNumber = UnityEngine.Random.Range(0, arr.Length);
             Player = arr[objNumber].transform;
             agent.SetDestination(Player.transform.position);
             timeLeft = 5;*/
            agent.SetDestination(Player1.transform.position);
        }
        
        if (Player == null)
        {
            agent.SetDestination(Player1.transform.position);
        }
       


    }
}
