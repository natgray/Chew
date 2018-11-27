using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace{
	public Vector2 gridPos;
	public Vector4 shape;
	public Vector2 prime;
	public GameObject prefab;
	public string furniture;
	public bool walkway, walkstart, occupied, isPrime;
	public int x, y;

	public GridSpace(Vector2 pos){
		gridPos = pos;
		walkway = occupied = walkstart = isPrime = false;
		furniture = "empty";
	}
	public GridSpace(Vector2 pos, bool ww, bool ws){
		gridPos = pos;
		walkway = occupied = ww;
		walkstart = ws;
		isPrime = false;
		if (ww) {
			furniture = "walkway";
		}
	}
}
