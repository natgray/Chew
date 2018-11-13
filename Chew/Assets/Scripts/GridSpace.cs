using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace{
	public Vector2 gridPos;
	public int neighbors;
	public bool walkway, walkstart, occupied;

	public GridSpace(Vector2 pos, int n){
		gridPos = pos;
		neighbors = n;
		walkway = occupied = walkstart = false;
	}
	public GridSpace(Vector2 pos, int n, bool ww, bool ws){
		gridPos = pos;
		neighbors = n;
		walkway = occupied = ww;
		walkstart = ws;

	}
}
