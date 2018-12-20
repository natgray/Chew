﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChewPtScript : MonoBehaviour {

	Renderer myRend;
	int chewColor = 0;
	public GameObject rabbit;
	Color farColor = Color.white;
	Color closeColor = Color.green;
	const float radius = 0.5f;

	void CheckPos(float x, float z)
	{
		float chewX = transform.position.x;
		float chewZ = transform.position.z;

		//get difference of Chew point and incoming object
		chewX = Mathf.Abs(chewX - x);
		chewZ = Mathf.Abs(chewZ - z);

		if (chewX < radius && chewX > -radius && chewZ < radius && chewZ > -radius)
		{
			myRend.material.SetColor("_Color", closeColor);
		} else
		{
			myRend.material.SetColor("_Color", farColor);
		}
	}

	// Use this for initialization
	void Start () {
		myRend = GetComponent<Renderer>();

		if (rabbit = GameObject.Find("Player"))
		{
			Debug.Log("passing");
		}
	}

	// Update is called once per frame
	void Update () {
		//Done: CheckPos() against the rabbit character here.
		CheckPos(rabbit.transform.position.x, rabbit.transform.position.z);
	}
}