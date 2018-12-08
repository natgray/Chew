using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour{
	public Vector2 gridPos;
	public Vector4 shape;
	public Vector2 prime;
	public GameObject prefab;
	public bool walkway, walkstart, occupied, isPrime, doorway;
	public int x, y;
}



/*
public GridSpace[,] markSpot(Vector2 shape, Vector2 spawn, GameObject prefab, string name, bool mustBeAnchoredX, bool mustBeAnchoredY, GridSpace [,] gridArray, int gridSizeX, int gridSizeY){
		List<Vector2> temp = new List<Vector2> ();
		temp.Add (spawn);
		return markSpot (shape, temp, prefab, name, mustBeAnchoredX, mustBeAnchoredY, gridArray, gridSizeX, gridSizeY);
	}
	public GridSpace[,] markSpot(Vector2 shape, List<Vector2> checks, GameObject prefab, string name, bool mustBeAnchoredX, bool mustBeAnchoredY, GridSpace [,] gridArray, int gridSizeX, int gridSizeY){
		List<Vector4> possible = new List<Vector4>();
		possible = findPossibles(shape, checks, gridArray, gridSizeX, gridSizeY);
		Vector4 choice = possible [Random.Range (0, possible.Count)]; // Pick one possible at random
		//while loop here
		//loop thru the axiss in question and see if its all on a wall
		// if not try another possible
		//if none of them work kick it back to findPossibles
		//maybe have a counter so it can run too much
		// Anchored means every gridspace along specified axis is abutting a wall
		bool anchors = false; // Whether the anchor conditions are met
		int counter = 0; // Stop trying to find valid positions 
		float check = 0F;
		if (mustBeAnchoredX) {
			check = shape.x;
		} else if (mustBeAnchoredY) {
			check = shape.y;
		} else {
			anchors = true;
		}
		while (possible.Count != 0 && !anchors && counter != 100) {
			if ((choice.x == 0 && choice.y == 0) || (choice.x == 0 && choice.y == gridSizeY - 1) || (choice.x == gridSizeX - 1 && choice.y == gridSizeY - 1) || (choice.x == gridSizeX - 1 && choice.y == 0)) {
				// corners are always anchored
				anchors = true;
				break;
			}
			int xlen = (int)Mathf.Abs (choice.x - choice.z) + 1; // get its x length (width)
			int ylen = (int)Mathf.Abs (choice.y - choice.w) + 1; // get its y length (height)
			anchors = true; 
			if (choice.x == 0 || choice.x == gridSizeX - 1) {
				//its on an x wall and you need to check its height
				if (ylen > check) {
					anchors = false;
				}
			}
			if (choice.y == 0 || choice.y == gridSizeY - 1) {
				//its on a y wall and you need to check its width
				if (xlen > check) {
					anchors = false;
				}
			}
			if (!anchors) {
				// its not anchored remove it from the list and get a new one
				possible.Remove(choice);
				if (possible.Count != 0) {
					choice = possible [Random.Range (0, possible.Count)];
				} else {
					return gridArray;
				}
			}
			counter++;
		}
		if (possible.Count != 0) {
			//Understand shape
			// then encode the information about it into the array
			int x, y;
			x = y = 0;
			if (choice.x <= choice.z) {
				x = 1;
			} else {
				x = -1;
			}
			if (choice.y <= choice.w) {
				y = 1;
			} else {
				y = -1;
			}
			// +/+
			if (x == 1 && y == 1) {
				for (int i = (int)choice.x; i <= (int)choice.z; i++) {
					for (int j = (int)choice.y; j <= (int)choice.w; j++) {
						gridArray [i, j].occupied = true;
						gridArray [i, j].isPrime = false;
						gridArray [i, j].prime = new Vector2(choice.x,choice.y);
						gridArray [i, j].shape = choice;
						gridArray [i, j].x = 1;
						gridArray [i, j].y = 1;
						gridArray [i, j].prefab = prefab;
						gridArray [i, j].furniture = name;
					}
				}
				gridArray[(int)choice.x,(int)choice.y].isPrime = true; // mark origin as prime
				return gridArray;
			}
			// +/-
			if (x == 1 && y == -1) {
				for (int i = (int)choice.x; i <= (int)choice.z; i++) {
					for (int j = (int)choice.w; j <= (int)choice.y; j++) {
						gridArray [i, j].occupied = true;
						gridArray [i, j].isPrime = false;
						gridArray [i, j].prime = new Vector2(choice.x,choice.y);
						gridArray [i, j].shape = choice;
						gridArray [i, j].x = 1;
						gridArray [i, j].y = -1;
						gridArray [i, j].prefab = prefab;
						gridArray [i, j].furniture = name;
					}
				}
				gridArray[(int)choice.x,(int)choice.y].isPrime = true; // mark origin as prime
				return gridArray;
			}
			//-/+
			if (x == -1 && y == 1) {
				for (int i = (int)choice.z; i <= (int)choice.x; i++) {
					for (int j = (int)choice.y; j <= (int)choice.w; j++) {
						gridArray [i, j].occupied = true;
						gridArray [i, j].isPrime = false;
						gridArray [i, j].prime = new Vector2(choice.x,choice.y);
						gridArray [i, j].shape = choice;
						gridArray [i, j].x = -1;
						gridArray [i, j].y = 1;
						gridArray [i, j].prefab = prefab;
						gridArray [i, j].furniture = name;
					}
				}
				gridArray[(int)choice.x,(int)choice.y].isPrime = true; // mark origin as prime
				return gridArray;
			}
			//--
			for (int i = (int)choice.z; i <= (int)choice.x; i++) {
				for (int j = (int)choice.w; j <= (int)choice.y; j++) {
					gridArray [i, j].occupied = true;
					gridArray [i, j].isPrime = false;
					gridArray [i, j].prime = new Vector2(choice.x,choice.y);
					gridArray [i, j].shape = choice;
					gridArray [i, j].x = -1;
					gridArray [i, j].y = -1;
					gridArray [i, j].prefab = prefab;
					gridArray [i, j].furniture = name;
				}
			}
			gridArray[(int)choice.x,(int)choice.y].isPrime = true; // mark origin as prime
			return gridArray;
		}
		return gridArray;
	}


*/