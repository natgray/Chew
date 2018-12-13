﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chewPtScript : MonoBehaviour {

	int chewColor = 0;
	public Material yellow;
	public Material red;
	public GameObject rabbit;

	void CheckPos(float x, float z)
	{
		float chewX = transform.position.x;
		float chewZ = transform.position.z;

		//get difference of Chew point and incoming object
		chewX = Mathf.Abs(chewX - x);
		chewZ = Mathf.Abs(chewZ - z);

		if (chewX < 5 && chewZ < 5)
		{
			GetComponent<Renderer>().material = red;
		} else
		{
			GetComponent<Renderer>().material = yellow;
		}
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//TODO: CheckPos() against the rabbit character here.
		CheckPos(rabbit.transform.position.x, rabbit.transform.position.z);
	}
}