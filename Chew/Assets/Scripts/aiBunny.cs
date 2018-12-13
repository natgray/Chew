using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiBunny : MonoBehaviour
{

    public NavMeshAgent agent;
    private Transform chewable;
    private Transform enemyDistance;
    public Transform Player1;
    float chewPointDistance;
    float enemydistance;
    public GameObject[] arr;
    float timeLeft = 5;
    public int objNumber;
    public float health = 100;
    public GameObject Bunny;
    public MeshRenderer bunnyMesh;

   

   

    //public GameObject[] chewables;
    // Use this for initialization
    void Start()
    {
        
        arr = GameObject.FindGameObjectsWithTag("chewable");
        //enemyHuman = GameObject.FindGameObjectWithTag("enemy");
       
        agent = GetComponent<NavMeshAgent>();

        objNumber = UnityEngine.Random.Range(0, arr.Length);

        chewable = arr[objNumber].transform;      

        if (chewable == null)
        {
            chewable = GameObject.FindWithTag("chewable").transform;
        }

        if (enemyDistance == null)
        {
            enemyDistance = GameObject.FindWithTag("enemy").transform;
        }
      
      

        //chewables = GameObject.FindWithTag("AIBunny").transform;
        agent.SetDestination(chewable.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        arr = GameObject.FindGameObjectsWithTag("chewable");
        chewPointDistance = Vector3.Distance(chewable.transform.position, transform.position);
        enemydistance = Vector3.Distance(enemyDistance.transform.position, transform.position);
        bunnyMesh = GetComponent<MeshRenderer>();
        if (chewPointDistance < 5)
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
        if (chewPointDistance > 10)
        {
            //animate.SetTrigger("walk");
        }

        if (enemydistance < 1)
        {
            health = health - 0.25f;
        }
        if (health == 0)
        {
            bunnyMesh.enabled = false;
            //Destroy(Bunny);

        }

    }
        
}
