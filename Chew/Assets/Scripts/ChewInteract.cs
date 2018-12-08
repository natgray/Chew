﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChewInteract : MonoBehaviour {

	public int[,] ChewPtTransform;
	//distance player can be from chew point to chew
	const float ChewRadius = 2;
	//amount of frames required to chew a point
	const int ChewHealth1 = 10000;

	// Use this for initialization
	void Start () {
		//Test data, Need actual chew point coordinate data.
		ChewPtTransform = new int[4, 4] { {25, 0, 25, ChewHealth1}, {-25, 0, 25, ChewHealth1}, {-25, 0, -25, ChewHealth1}, {25, 0, -25, ChewHealth1} };
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E))
		{
			int x = 0;
			while (x < ChewPtTransform.GetLength(0))
			{
				if (CheckPt(ChewPtTransform[x, 0], ChewPtTransform[x, 1], ChewPtTransform[x, 2], x))
				{
					break;
				}
				x++;
			}
		}
	}

	bool CheckPt (int x, int y, int z, int index)
	{
		//rabbit within radius
		if ((Mathf.Abs(gameObject.transform.position.x - x) < ChewRadius) && (Mathf.Abs(gameObject.transform.position.y - y) < ChewRadius) && (Mathf.Abs(gameObject.transform.position.z - z) < ChewRadius))
		{
			//TODO: check to see if user is looking at chew point
			ChewPtTransform[index, 3]++;
			return true;
		} else
		{
			return false;
		}
	}

}