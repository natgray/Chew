using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Strategy
 * This would get attached to a room that has been deterimined to be a bedroom
 * There will be a bed in it already, as well as designated path area
 * Things to do to fill it out:
 * Nightstands next to the bed
 * bench at end of bed
 * Dresser + mirrior
 * Desk + chair
 * sitting area
 * laundry basket/backpack/other clutter on floor thats fodder for rabbits
 * Lights
 * spawn walls/ceiling
 * Would then kick it over to a prop script which kicks it over to a finalizing script
 * 
 * Potential pitfalls
 * Ensure suffiecent spaces for destructibles to be spawned
 * 
 * Ideas:
 * Mark area immediatly next to the bed as pathway, connect to rest of pathway
 * */

public class BedroomGenerator : MonoBehaviour {
	public GridSpace[,] spaces;
	int gridSizeX, gridSizeY;
	public GameController gamecontroller;

	// Use this for initialization
	void Start () {
		spaces = gameObject.GetComponent<Room>().spaces;
		gridSizeX = gameObject.GetComponent<Room> ().gridSizeX;
		gridSizeY = gameObject.GetComponent<Room> ().gridSizeY;
		generateFurniture ();
		//gameObject.GetComponent<Room> ().DrawMap ();
		gameObject.GetComponent<Room> ().SpawnFurniture ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void generateFurniture(){
		//try adding a nightstand next to the bed
		//find the bed
		GridSpace bed = null;
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				if (s.furniture.Equals ("bed")) {
					bed = s;
				}
			}
		}
		if (bed != null) {
			//Try spawning two night stands
			//Understand shape
			List<Vector2> nightStands= new List<Vector2>();
			if (Mathf.Abs (bed.shape.x - bed.shape.z) > Mathf.Abs(bed.shape.y - bed.shape.w)) {
				//if this is true then the short axis is the Y
				if (bed.y > 0) {
					nightStands.Add (new Vector2 (bed.gridPos.x, bed.gridPos.y+2));
					nightStands.Add (new Vector2 (bed.gridPos.x, bed.gridPos.y-1));
				} else {
					nightStands.Add (new Vector2 (bed.gridPos.x, bed.gridPos.y-2));
					nightStands.Add (new Vector2 (bed.gridPos.x, bed.gridPos.y+1));
				}
			} else {
				// short axis is the X
				if (bed.x > 0) {
					nightStands.Add (new Vector2 (bed.gridPos.x+2, bed.gridPos.y));
					nightStands.Add (new Vector2 (bed.gridPos.x-1, bed.gridPos.y));
				} else {
					nightStands.Add (new Vector2 (bed.gridPos.x-2, bed.gridPos.y));
					nightStands.Add (new Vector2 (bed.gridPos.x+1, bed.gridPos.y));
				}
			}
			foreach (Vector2 v in nightStands) {
				int x = (int)v.x;
				int y = (int)v.y;
				if (!(v.x >= gridSizeX) && !(v.y >= gridSizeY) && !(v.x <0) && !(v.y <0) &&!(spaces [x, y].occupied)) {
					gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (1, 1), v, gamecontroller.GetComponent<PreFabLibrary> ().nightstand, "nightstand", false, false, spaces, gridSizeX, gridSizeY);  
				}
			}
		} 
		//find another spot and spawn a dresser
		gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (3, 1), gamecontroller.GetComponent<RoomGenerator>().generateCheckList(true, gridSizeX, gridSizeY), gamecontroller.GetComponent<PreFabLibrary> ().dresser, "dresser", true, false, spaces, gridSizeX, gridSizeY);
		// find another spot and spawn a desk
		gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (2, 2), gamecontroller.GetComponent<RoomGenerator>().generateCheckList(true, gridSizeX, gridSizeY), gamecontroller.GetComponent<PreFabLibrary> ().deskandchair, "desk and chair", true, false, spaces, gridSizeX, gridSizeY);
		// maybe a book shelf, space permitting
		for (int x = 0; x < Random.Range (1, 5); x++) {
			gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (1, 1), gamecontroller.GetComponent<RoomGenerator> ().generateCheckList (true, gridSizeX, gridSizeY), gamecontroller.GetComponent<PreFabLibrary> ().getBookShelf (), "bookshelf", true, false, spaces, gridSizeX, gridSizeY);
		}
		gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (1, 1), gamecontroller.GetComponent<RoomGenerator>().generateCheckList(true, gridSizeX, gridSizeY), gamecontroller.GetComponent<PreFabLibrary> ().smalltable, "small table", true, false, spaces, gridSizeX, gridSizeY);
		GridSpace smtable = null;
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				if (s.furniture.Equals ("small table")) {
					smtable = s;
				}
			}
		}
		if (smtable != null) {
			//space permitting spawn two chairs next to it
			// find the 8 spaces around the table and pick up to two to spawn chairs
			List<Vector2> chairs = new List<Vector2>();
			chairs.Add (new Vector2 (smtable.gridPos.x-1, smtable.gridPos.y)); // left
			chairs.Add (new Vector2 (smtable.gridPos.x+1, smtable.gridPos.y)); // right
			chairs.Add (new Vector2 (smtable.gridPos.x, smtable.gridPos.y+1)); // up
			chairs.Add (new Vector2 (smtable.gridPos.x, smtable.gridPos.y-1)); //down
			chairs.Add (new Vector2 (smtable.gridPos.x-1, smtable.gridPos.y+1)); // up left
			chairs.Add (new Vector2 (smtable.gridPos.x+1, smtable.gridPos.y+1)); // up right
			chairs.Add (new Vector2 (smtable.gridPos.x-1, smtable.gridPos.y-1)); // down left
			chairs.Add (new Vector2 (smtable.gridPos.x+1, smtable.gridPos.y-1)); // down right

			int chairLimit = 2;
			int chairsSpawn = 0;
			foreach (Vector2 v in chairs) {
				if (chairsSpawn < chairLimit && v.x >= 0 && v.y >= 0 && v.x < gridSizeX && v.y < gridSizeY && !(spaces[(int)v.x,(int)v.y].occupied)) {
					//gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (1, 1), v, gamecontroller.GetComponent<PreFabLibrary> ().chair, "chair", false, false, spaces, gridSizeX, gridSizeY);
					chairsSpawn++;
				}
			}
		}
		gamecontroller.GetComponent<RoomGenerator> ().markSpot (new Vector2 (1, 1), gamecontroller.GetComponent<RoomGenerator>().generateCheckList(true, gridSizeX, gridSizeY), gamecontroller.GetComponent<PreFabLibrary> ().floorlamp, "floor lamp", true, false, spaces, gridSizeX, gridSizeY);



		//end of furniture generation
	}

	// end of class
}
