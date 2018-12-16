using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiBunny : MonoBehaviour
{

    public NavMeshAgent agent;
	private GameObject chewable;
    private Transform enemyDistance;
    public GameObject humanAI;
    float chewPointDistance;
    float enemydistance;
    float timeLeft = 5;
    public float health = 100;
    public GameObject Bunny;
	public GameController gamecontroller;
   

   

    //public GameObject[] chewables;
    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (enemyDistance == null)
        {
            enemyDistance = GameObject.FindWithTag("enemy").transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
		if (chewable == null)
		{
			Debug.Log (gameObject.name + ": chewable was null.");
			chewable = gamecontroller.getRandomDestructionSpot(); 
			agent.SetDestination(chewable.transform.position);
		}
        chewPointDistance = Vector3.Distance(chewable.transform.position, transform.position);
        enemydistance = Vector3.Distance(enemyDistance.transform.position, transform.position);
        if (chewPointDistance < 5)
        {
            //animate.SetTrigger("attack");
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
				chewable = gamecontroller.getRandomDestructionSpot();
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
        if (health < -60)
        {
			humanAI.GetComponent<aiScript> ().deregisterBunny (gameObject);
            Destroy(gameObject);

        }

    }
        
}
