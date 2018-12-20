using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiScript : MonoBehaviour {
	public Transform target;
    public NavMeshAgent agent;
	public GameObject Player;

    //Animator animate;
    float distance;
    float distanceToPlayer;
    int health = 10;
    public int playerHealth = 500;
    public List<GameObject> bunnies;
    public float timeLeft = 5;
	public float timeOut = 45;


    // Use this for initialization
    void Start () {
        //animate = GetComponent<Animator>();
        //animate.SetTrigger("walk");
		bunnies = new List<GameObject>();
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject b in temp) {
			bunnies.Add (b);
		}
		target = bunnies[Random.Range(0, bunnies.Count)].transform;
		 agent = GetComponent<NavMeshAgent>();
        //Player = GameObject.FindWithTag("Player").transform;
        agent.SetDestination(target.transform.position);
        //col = GameObject.FindWithTag("col");
        //Collider collider = GetChild(0).GetComponent<Collider>();
        distance = Vector3.Distance(target.position, transform.position);

    }

    // Update is called once per frame
    void Update () {
		if (target == null) {
			target = bunnies[Random.Range(0, bunnies.Count)].transform;
		}
		timeOut -= Time.deltaTime;
		if (timeOut < 0) {
			target = bunnies[Random.Range(0, bunnies.Count)].transform;
			timeOut = 45;
		}
        distance = Vector3.Distance(target.position, transform.position);
		distanceToPlayer = Vector3.Distance (Player.transform.position, transform.position);
        if (distance < 2){
            timeLeft -= Time.deltaTime;
        }
		if (distanceToPlayer < 2){
			playerHealth -= 1;
		}
       
        if (timeLeft < 0) {
			target = bunnies[Random.Range(0, bunnies.Count)].transform;
            agent.SetDestination(target.transform.position);
            timeLeft = 5;
			timeOut = 45;
            Debug.Log("Picking new target");
        }

        if (timeLeft > 0 && timeLeft < 5){
            //agent.SetDestination(Player.transform.position);
            /* objNumber = UnityEngine.Random.Range(0, arr.Length);
             Player = arr[objNumber].transform;
             agent.SetDestination(Player.transform.position);
             timeLeft = 5;*/
            agent.SetDestination(target.transform.position);
        }
        
        if (target == null)
        {
            agent.SetDestination(target.transform.position);
        }
       


    }

	public void deregisterBunny(GameObject bunny){
		bunnies.Remove (bunny);
	}
}
