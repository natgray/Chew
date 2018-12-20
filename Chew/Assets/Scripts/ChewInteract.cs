using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChewInteract : MonoBehaviour
{

    public ChewPt[] ChewPtTransform;
    //distance player can be from chew point to chew
    const float ChewRadius = 0.5f;
    //amount of frames required to chew a point
    public GameController gameControlObj;
    public GameObject Player;

    public GameObject destroyedPoint;
    Mesh destroyedMesh;

    int[] chewHpArray;

    // Use this for initialization
    void Start()
    {
        //ready the destroyed mesh

        //make array to reference points
        chewHpArray = new int[15];

        //I do this line right?
        //gameControlObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        //getting chew point data
        int x = 0;
        Debug.Log("aaa");
        while (x < 15)
        {
            chewHpArray[x] = 1;
            Debug.Log("adding chewpt");
            x++;
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButton (0))
        {
            int x = 0;
            while (x < gameControlObj.destructionSpots.Count)
            {
                //TODO: add buffer that checks the last successful point before checking all points
                //
                if (CheckPt(gameControlObj.destructionSpots[x].transform.position.x, gameControlObj.destructionSpots[x].transform.position.y, gameControlObj.destructionSpots[x].transform.position.z, x))
                {
                    Debug.Log("foundpoint");
                }
                x++;
            }
        }
    }

    bool CheckPt(float x, float y, float z, int index)
    {
        //rabbit within radius
        if ((Mathf.Abs(Player.transform.position.x - x) < ChewRadius) && (Mathf.Abs(Player.transform.position.y - y) < ChewRadius) && (Mathf.Abs(Player.transform.position.z - z) > 0))
        {
            chewHpArray[index]--;
            Debug.Log("chewing" + chewHpArray[index]);
            if (chewHpArray[index] <= 0)
            {
                gameControlObj.destructionSpots[index].GetComponent<MeshRenderer>().enabled = false;
				gameControlObj.deregeisterDestructionSpot (gameControlObj.destructionSpots [index]);
            }
            //gameControlObj.destructionSpots[index].GetComponent<MeshFilter>().mesh = destroyedMesh;
            return true;
        }
        else
        {
            return false;
        }
    }

}

public class ChewPt
{
    public int ptHp;
    public GameObject chewPt;

    void Start()
    {
        ptHp = 1;
    }
}
