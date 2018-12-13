﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChewInteract : MonoBehaviour {

	public ChewPt[] ChewPtTransform;
	//distance player can be from chew point to chew
	const float ChewRadius = 2;
	//amount of frames required to chew a point
	public GameController gameControlObj;

	public GameObject destroyedPoint;
	Mesh destroyedMesh;

	// Use this for initialization
	void Start () {
		//ready the destroyed mesh
		destroyedMesh = destroyedPoint.GetComponent<MeshFilter>().mesh;

		//make array to reference points
		ChewPtTransform = new ChewPt[gameControlObj.destructionSpots.Count];

		//I do this line right?
		gameControlObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

		//getting chew point data
		int x = 0;
		while (x < gameControlObj.destructionSpots.Count)
		{
			ChewPtTransform[x].chewPt = gameControlObj.destructionSpots[x];
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E))
		{
			int x = 0;
			while (x < ChewPtTransform.GetLength(0))
			{
				//TODO: add buffer that checks the last successful point before checking all points
				//
				if (ChewPtTransform[x].ptHp > 0 && CheckPt(ChewPtTransform[x].chewPt.transform.position.x, ChewPtTransform[x].chewPt.transform.position.y, ChewPtTransform[x].chewPt.transform.position.z, x))
				{
					break;
				}
				x++;
			}
		}
	}

	bool CheckPt (float x, float y, float z, int index)
	{
		//rabbit within radius
		if ((Mathf.Abs(gameObject.transform.position.x - x) < ChewRadius) && (Mathf.Abs(gameObject.transform.position.y - y) < ChewRadius) && (Mathf.Abs(gameObject.transform.position.z - z) > 0))
		{
			//done: check to see if user is looking at chew point
			ChewPtTransform[index].ptHp--;
			//todo: change object based on hp value
			if (ChewPtTransform[index].ptHp <= 0)
			{
				gameControlObj.destructionSpots[index].GetComponent<MeshFilter>().mesh = destroyedMesh;
			}

			return true;
		} else
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
		ptHp = 25500;
	}
}