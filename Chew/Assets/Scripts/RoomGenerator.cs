using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
	// Temp variables for creating a visual map
	public GameObject prefab, prefabBed, prefabDresser, prefabcouch;
	public Transform testarea;
	public int oneXoneSize;
	public GameObject [] oneXonePrefabs;
	Vector2 roomSize;
	int patharea;
	// Setup the Room Size
	public int gridSizeX;
	public int gridSizeY;
	GridSpace[,] spaces; // an array to keep info about potential spaces in the room
	List<Vector2> takenPositions = new List<Vector2> (); // List of cells that are marked as taken

	void Start(){
		//oneXonePrefabs = new GameObject[oneXoneSize];
		if (gridSizeX <= 0) {
			gridSizeX = 4;
		}
		if (gridSizeY <= 0) {
			gridSizeY = 4;
		}
		roomSize = new Vector2(gridSizeX,gridSizeY); // Initlize the room size
		spaces = new GridSpace[gridSizeX,gridSizeY]; // set the spaces array to the proper size
		patharea = Mathf.RoundToInt((gridSizeX * gridSizeY)/3); // The number of cells to mark as walkway, by default mark about 1/3rd of the area to be walkway
		//Make sure we aren't trying to create more walkway than we have space
		if (patharea > roomSize.x *roomSize.y) {  // If theres more lower it to be half the total cells availible
			patharea = Mathf.RoundToInt(roomSize.x  * roomSize.y);
		}
		CreatePathways (); //This procedurally generates a pathway 
		FillavailblePositions ();
		DrawMap ();
		populateRoom ();
	}

	void CreatePathways(){
		//  values to govern the generation
		float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.1f;
		int desiredNeighbors = 1;

		Vector2 start = new Vector2 (0, Mathf.RoundToInt(gridSizeY/2)); //Where to start the path, by default start halfway down the left side

		spaces [(int)start.x, (int) start.y] = new GridSpace(start, 0, true, true); // generate one peice of path to start things off
		takenPositions.Add (start); //Add start to the list of taken spaces

		Vector2 checkPos = start; // dont think we need to do this
		//add pathway
		for (int i = 0; i < patharea - 1; i++) {
			float randomPerc = ((float)i) / (((float)patharea - 1));
			randomCompare = Mathf.Lerp (randomCompareStart, randomCompareEnd, randomPerc); //The farther into the loop, the less likely new branches will be created

			checkPos = NewPosition (); //get a new position

			//test the new position
			if (numberOfNeighbors (checkPos, takenPositions) > desiredNeighbors && Random.value > randomCompare) { //Find the number of neighboring occupied cells, if its more than desiredNeighbors find somewhere else, unless we randomly skip this step
				int x = 0;  //counter
				do {
					checkPos = SelectiveNewPosition (desiredNeighbors); // Find a new cell to check
					x++;
				} while(numberOfNeighbors (checkPos, takenPositions) > desiredNeighbors && x < 100); //Give the loop 100 tries to find an approriate cell 
				if (x == 100) {
					Debug.Log ("Could not find a cell with less than the desired Neighbors(" + desiredNeighbors + ")");
				}
			}
			// Finalize position
			spaces[(int) checkPos.x, (int) checkPos.y] = new GridSpace(checkPos, -1, true, false); // add the new peice of walkway to the spaces and mark it as walkway
			takenPositions.Add (checkPos); // mark the position as taken
		}
		// Now that all that paths are generated find neighbors
		foreach (GridSpace s in spaces) {
			if (s != null) {
				s.neighbors = numberOfNeighbors (s.gridPos, takenPositions);
			}
		}
	}
	//function to find new valid positions
	Vector2 NewPosition(){
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do {
			//get a random position that's already taken
			int index = Random.Range(0,takenPositions.Count);
			x = (int)takenPositions [index].x;
			y = (int)takenPositions [index].y;
			//Random select a direction to head in 
			// Note!!!!!! maybe this should include diag!!!!!
			int dir = Random.Range(0,4);
			if(dir == 0){
				y++;
			}
			if(dir == 1){
				x++;
			}
			if(dir == 2){
				y--;
			}
			if(dir == 3){
				x--;
			}
			checkingPos = new Vector2 (x, y);
		} while(takenPositions.Contains (checkingPos) || x < 0 || y < 0 || x >= gridSizeX || y >= gridSizeY ); // loop until we find a valid non-taken position
		return checkingPos;
	}
	//function to find new valid positions with less than desired neighbors
	Vector2 SelectiveNewPosition(int neighbors){
		int index = 0, inc = 0;
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do {
			inc = 0;
			do {
				index = Mathf.RoundToInt(Random.value *(takenPositions.Count -1));
				inc++;
			} while(numberOfNeighbors (takenPositions[index], takenPositions) > neighbors && inc < 100); //Give the loop 100 tries to find an approriate cell
			if(inc >= 100){
				Debug.Log("SelectiveNewPosition, Could not find an approriate cell");
			}
			//get a random position that's already taken
			index = Random.Range(0,takenPositions.Count);
			x = (int)takenPositions [index].x;
			y = (int)takenPositions [index].y;
			//Random select a direction to head in
			int dir = Random.Range(0,4);
			if(dir == 0){
				y++;
			}
			if(dir == 1){
				x++;
			}
			if(dir == 2){
				y--;
			}
			if(dir == 3){
				x--;
			}
			checkingPos = new Vector2 (x, y);
		} while(takenPositions.Contains (checkingPos) || x < 0 || y < 0 || x >= gridSizeX || y >= gridSizeY ); // loop until we find a non-taken valid position
		return checkingPos;
	}

	int numberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions){
		int ret = 0;
		if (usedPositions.Contains (checkingPos + Vector2.right)) {
			ret++;
		}
		if (usedPositions.Contains (checkingPos + Vector2.left)) {
			ret++;
		}
		if (usedPositions.Contains (checkingPos + Vector2.up)) {
			ret++;
		}
		if (usedPositions.Contains (checkingPos + Vector2.down)) {
			ret++;
		}
		return ret;

	}
	void FillavailblePositions(){
		// Loop the array, anywhere there isn't a walkway add an empty space
		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector2 temp = new Vector2 (x, y);
				if (!takenPositions.Contains(temp)) {
					spaces [x, y] = new GridSpace (temp, 4-numberOfNeighbors(temp,takenPositions));
					//check for edges
					if (spaces [x, y].gridPos.x == 0 || spaces [x, y].gridPos.x == gridSizeX - 1) { // means its on the left or right egde
						spaces [x, y].neighbors--;
					}
					if (spaces [x, y].gridPos.y == 0 || spaces [x, y].gridPos.y == gridSizeY - 1) { // means its on the top or bottom edge
						spaces [x, y].neighbors--;
					}
				}
			}
		}
	}

	void DrawMap(){
		foreach (GridSpace s in spaces) {
			if (!s.walkway) {
				GameObject whiteCube = Object.Instantiate (prefab, Vector3.zero, Quaternion.identity);
				whiteCube.transform.SetParent (testarea);
				whiteCube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				whiteCube.GetComponent<Renderer> ().material.color = Color.white;
				whiteCube.GetComponent<TestCube> ().x = s.gridPos.x;
				whiteCube.GetComponent<TestCube> ().y = s.gridPos.y;
				whiteCube.GetComponent<TestCube> ().neighbors = s.neighbors;
			} else {
				GameObject cube = Object.Instantiate (prefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (testarea);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.cyan;
				cube.GetComponent<TestCube> ().x = s.gridPos.x;
				cube.GetComponent<TestCube> ().y = s.gridPos.y;
				cube.GetComponent<TestCube> ().neighbors = s.neighbors;
			}
		}
		//testarea.position = new Vector3 (-185, 20, -42); //
	}

	void populateRoom(){
		//strategy
		//Randomly select edges and try to place some large, room defining feature
		//include random skip
		//at first pass, that defines the room
		//then kick it over to a more specilized function
		//Start with a bed
		Vector2 bed = new Vector2 (3, 2);
		Vector2 endtable = new Vector2 (1, 1);
		Vector2 couch = new Vector2 (3, 1);
		Vector2 dresser = new Vector2 (2, 1);
		//List<Vector4> possible = new List<Vector4>();
		List<Vector2> checks = generateCheckList (true);
		//spawn some stuff
		// need to mark stuff as taken
		pickAndSpawn(bed,checks, prefabBed);
		checks = generateCheckList (true);
		pickAndSpawn (couch, checks, prefabcouch);
		checks = generateCheckList (true);
		pickAndSpawn (dresser, checks, prefabDresser);
		checks = generateCheckList (false);
		for (int i = 0; i < 6; i++) {
			pickAndSpawn (endtable, checks, oneXonePrefabs [Random.Range (0, oneXoneSize)]);
		}

	}

	List<Vector2> generateCheckList(bool wallsOnly){
		List<Vector2> checks = new List<Vector2> ();
		if (wallsOnly) {
			int attemptsPerWall = 3;
			// add some on the x0 wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x = 0;
				int y = Random.Range (0, gridSizeY);
				checks.Add (new Vector2 (x, y));
			}
			// add some on y0 wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x =  Random.Range (0, gridSizeX);
				int y = 0;
				checks.Add (new Vector2 (x, y));
			}
			//add some on xMax wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x = gridSizeX-1;
				int y = Random.Range (0, gridSizeY);
				checks.Add (new Vector2 (x, y));
			}
			//add some on the yMax wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x =  Random.Range (0, gridSizeX);
				int y = gridSizeY-1;
				checks.Add (new Vector2 (x, y));
			}
			return checks;
		}
		for (int i = 0; i < 10; i++) {
			checks.Add (new Vector2 (Random.Range (0, gridSizeX), Random.Range (0, gridSizeY)));
		}
		return checks;
	}
	void pickAndSpawn(Vector2 shape, List<Vector2> checks, GameObject prefab){
		List<Vector4> possible = new List<Vector4>();
		possible = findPossibles(shape, checks);
		if (possible.Count != 0) {
			Vector4 choice = possible [Random.Range (0, possible.Count)];
			//Debug.Log (choice);
			GameObject furniture = Object.Instantiate (prefab, Vector3.zero, Quaternion.Euler(-90,0,0));
			furniture.transform.SetParent (testarea);
			furniture.transform.localPosition = (new Vector3 (choice.x, 1, choice.y)); 
			markAsTaken (choice);
		}
	}
	void markAsTaken(Vector4 shape){
		//Understand shape
		int x, y;
		x = y = 0;
		if (shape.x <= shape.z) {
			x = 1;
		} else {
			x = -1;
		}
		if (shape.y <= shape.w) {
			y = 1;
		} else {
			y = -1;
		}
		// +/+
		if (x == 1 && y == 1) {
			for (int i = (int)shape.x; i <= (int)shape.z; i++) {
				for (int j = (int)shape.y; j <= (int)shape.w; j++) {
					spaces [i, j].occupied = true;
				}
			}
			return;
		}
		// +/-
		if (x == 1 && y == -1) {
			for (int i = (int)shape.x; i <= (int)shape.z; i++) {
				for (int j = (int)shape.w; j <= (int)shape.y; j++) {
					spaces [i, j].occupied = true;
				}
			}
			return;
		}
		//-/+
		if (x == -1 && y == 1) {
			for (int i = (int)shape.z; i <= (int)shape.x; i++) {
				for (int j = (int)shape.y; j <= (int)shape.w; j++) {
					spaces [i, j].occupied = true;
				}
			}
			return;
		}
		//--
		for (int i = (int)shape.z; i <= (int)shape.x; i++) {
			for (int j = (int)shape.w; j <= (int)shape.y; j++) {
				spaces [i, j].occupied = true;
			}
		}
		return;
	}
	List<Vector4> findPossibles(Vector2 space, List<Vector2> checks){
		List<Vector4> possible = new List<Vector4> ();
		foreach(Vector2 c in checks){
			List<Vector2> results = new List<Vector2> ();
			results = findSpaces (c, (int)space.x, (int)space.y, gridSizeX, gridSizeY, false);
			foreach (Vector2 v in results) {
				possible.Add (new Vector4(c.x,c.y,v.x,v.y));
			}
		}
		// Try again but with the flipped dimensions (if not a square)
		if (space.y != space.x) {
			foreach (Vector2 c in checks) {
				List<Vector2> results = new List<Vector2> ();
				results = findSpaces (c, (int)space.y, (int)space.x, gridSizeX, gridSizeY, false);
				foreach (Vector2 v in results) {
					possible.Add (new Vector4 (c.x, c.y, v.x, v.y));
				}
			}
		}
		return possible;
	}

	List<Vector2> findSpaces(Vector2 start, int height, int width, int totalWidth, int totalHeight, bool verbose){
		List<Vector2> results = new List<Vector2>();
		if (verbose) {Debug.Log ("Searching for " + width + "x" + height + " large space starting from: (" + start.x + "," + start.y + ")");}
		if (spaces [(int)start.x, (int)start.y].occupied) {
			if (verbose) {Debug.Log ("Space: " + start + " is a walkway");}
			return results;
		}
		bool possible = true;
		// check +/+
		if ((start.x + (width - 1) >= totalWidth) || (start.y + (height - 1) >= totalHeight)) {
			possible = false;
			if (verbose) {Debug.Log ("+/+ availble space failed");}
		} else{
			if (verbose) {Debug.Log ("trying +/+");}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (spaces [((int)start.x) + x, ((int)start.y) + y].occupied) {
						possible = false;
						break;
					}
				}
			}
		}
		if (possible) {
			if (verbose) {Debug.Log ("+/+ found");}
			float x = start.x + (width - 1);
			float y = start.y + (height - 1);
			results.Add (new Vector2 (x,y));
		}
		//check -/-
		possible = true;
		if ((start.x - (width-1) < 0) || (start.y - (height -1) < 0)) {
			possible = false;
			if (verbose) {Debug.Log ("-/- availble space failed");}
		} else {
			if (verbose) {Debug.Log ("trying -/-");}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (spaces [((int)start.x) - x, ((int)start.y) - y].occupied) {
						possible = false;
						break;
					}
				}
			}
		}
		if (possible) {
			if (verbose) {Debug.Log ("-/- found");}
			float x = start.x - (width - 1);
			float y = start.y - (height - 1);
			results.Add (new Vector2 (x,y));
		}
		//check +/-
		possible = true;
		if ((start.x + (width - 1) >= totalWidth) || (start.y - (height +1) < 0)) {
			possible = false;
			if (verbose) {Debug.Log ("+/- availble space failed");}
		} else {
			if (verbose) {Debug.Log ("trying +/-");}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (spaces [((int)start.x) + x, ((int)start.y) - y].occupied) {
						possible = false;
						break;
					}
				}
			}
		}
		if (possible) {
			if (verbose) {Debug.Log ("+/- found");}
			float x = start.x + (width - 1);
			float y = start.y - (height - 1);
			results.Add (new Vector2 (x,y));
		}
		//check -/+
		possible = true;
		if ((start.x - (width-1) < 0 )|| (start.y + (height - 1) >= totalHeight)) {
			possible = false;
			if (verbose) {Debug.Log ("-/+ availble space failed");}
		} else {
			if (verbose) {Debug.Log ("trying -/+");}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (spaces [((int)start.x) - x, ((int)start.y) + y].occupied) {
						possible = false;
						break;
					}
				}
			}
		}
		if (possible) {
			if (verbose) {Debug.Log ("-/+ found");}
			float x = start.x - (width - 1);
			float y = start.y + (height - 1);
			results.Add (new Vector2 (x,y));
		}
		return results;
	}
	//end of class
}
