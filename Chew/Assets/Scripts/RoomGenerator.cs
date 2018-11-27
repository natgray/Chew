using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Next step is to deterimine rotation objects should have
// Then make them line up to the right cells
// Then start adding rules
public class RoomGenerator : MonoBehaviour {
	// Temp array of test objects
	public int oneXoneSize;
	public GameObject [] oneXonePrefabs;
	GridSpace[,] spaces; // an array to keep info about potential spaces in the room
	Vector2 roomSize;
	int patharea;
	// Setup the Room Size
	public int gridSizeX;
	public int gridSizeY;
	List<Vector2> takenPositions; // List of cells that are marked as taken

	void Start(){
	}

	public void GenerateRoom(int SizeX, int SizeY, Transform roomTransform){
		gridSizeY = SizeY;
		gridSizeX = SizeX;
		if (gridSizeX <= 0) {
			gridSizeX = 4;
		}
		if (gridSizeY <= 0) {
			gridSizeY = 4;
		}
		spaces = new GridSpace[gridSizeX,gridSizeY]; // set the spaces array to the proper size
		roomSize = new Vector2(gridSizeX,gridSizeY); // Initlize the room size
		takenPositions = new List<Vector2> (); // Initlize the takenPosition list
		patharea = Mathf.RoundToInt((gridSizeX * gridSizeY)/3); // The number of cells to mark as walkway, by default mark about 1/3rd of the area to be walkway
		//Make sure we aren't trying to create more walkway than we have space
		if (patharea > roomSize.x *roomSize.y) {  // If theres more lower it to be half the total cells availible
			patharea = Mathf.RoundToInt(roomSize.x  * roomSize.y);
		}
		roomTransform.GetComponent<Room> ().gridSizeX = gridSizeX;
		roomTransform.GetComponent<Room> ().gridSizeY = gridSizeY;
		CreatePathways (); //This procedurally generates a pathway 
		FillavailblePositions ();
		populateRoom ();
		roomTransform.GetComponent<Room> ().spaces = spaces;
	}
	void CreatePathways(){
		//  values to govern the generation
		float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.1f;
		int desiredNeighbors = 1;

		Vector2 start = new Vector2 (0, Mathf.RoundToInt(gridSizeY/2)); //Where to start the path, by default start halfway down the left side

		spaces [(int)start.x, (int) start.y] = new GridSpace(start, true, true); // generate one peice of path to start things off
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
			spaces[(int) checkPos.x, (int) checkPos.y] = new GridSpace(checkPos, true, false); // add the new peice of walkway to the spaces and mark it as walkway
			takenPositions.Add (checkPos); // mark the position as taken
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
					spaces [x, y] = new GridSpace (temp);
				}
			}
		}
	}
		
	void populateRoom(){
		//strategy
		//Randomly select edges and try to place some large, room defining feature
		Vector2 bed = new Vector2 (3, 2);
		Vector2 endtable = new Vector2 (1, 1);
		Vector2 couch = new Vector2 (3, 1);
		Vector2 dresser = new Vector2 (2, 1);
		List<Vector2> checks = generateCheckList (true, gridSizeX, gridSizeY);
		markSpot(bed,checks, gameObject.GetComponent<PreFabLibrary>().bed, "bed", false, true, spaces, gridSizeX, gridSizeY);
		//checks = generateCheckList (true);
		//pickAndMark (couch, checks, prefabcouch, "couch");
		//checks = generateCheckList (true);
		//pickAndMark (dresser, checks, prefabDresser, "dresser");
		//checks = generateCheckList (false);
		for (int i = 0; i < 6; i++) {
		//	pickAndMark (endtable, checks, oneXonePrefabs [Random.Range (0, oneXoneSize)], "small");
		}

	}

	public List<Vector2> generateCheckList(bool wallsOnly, int gridSizeX, int gridSizeY){
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
				int x = Random.Range (0, gridSizeX);
				int y = 0;
				checks.Add (new Vector2 (x, y));
			}
			//add some on xMax wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x = gridSizeX - 1;
				int y = Random.Range (0, gridSizeY);
				checks.Add (new Vector2 (x, y));
			}
			//add some on the yMax wall
			for (int i = 0; i < attemptsPerWall; i++) {
				int x = Random.Range (0, gridSizeX);
				int y = gridSizeY - 1;
				checks.Add (new Vector2 (x, y));
			}
			return checks;
		} else {
			for (int i = 0; i < 10; i++) {
				checks.Add (new Vector2 (Random.Range (0, gridSizeX), Random.Range (0, gridSizeY)));
			}
		}
		return checks;
	}
	List<Vector2> findSpaces(Vector2 start, int height, int width, int totalWidth, int totalHeight,bool verbose, GridSpace [,] gridArray){
		List<Vector2> results = new List<Vector2>();
		if (verbose) {Debug.Log ("Start is: " + start + ". height is: " + height + " width is: " + width + "total width is: " + totalWidth + "total height is: " + totalHeight);}
		if (verbose) {Debug.Log ("Searching for " + width + "x" + height + " large space starting from: (" + start.x + "," + start.y + ")");}
		if (gridArray [(int)start.x, (int)start.y].occupied) {
			if (verbose) {Debug.Log ("Space: " + start + " is a walkway");}
			return results;
		}
		bool possible = true;
		// check +/+
		if (verbose) {Debug.Log ("Checking +/+");}
		if ((start.x + (width - 1) >= totalWidth) || (start.y + (height - 1) >= totalHeight)) {
			possible = false;
			if (verbose) {Debug.Log ("+/+ availble space failed");}
		} else{
			if (verbose) {Debug.Log ("trying +/+");}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (gridArray [((int)start.x) + x, ((int)start.y) + y].occupied) {
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
					if (gridArray [((int)start.x) - x, ((int)start.y) - y].occupied) {
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
					if (gridArray [((int)start.x) + x, ((int)start.y) - y].occupied) {
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
					if (gridArray [((int)start.x) - x, ((int)start.y) + y].occupied) {
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

	List<Vector4> findPossibles(Vector2 space, List<Vector2> checks, GridSpace [,] gridArray, int gridSizeX, int gridSizeY){
		List<Vector4> possible = new List<Vector4> ();
		foreach(Vector2 c in checks){
			List<Vector2> results = new List<Vector2> ();
			results = findSpaces (c, (int)space.x, (int)space.y, gridSizeX, gridSizeY, false, gridArray);
			foreach (Vector2 v in results) {
				possible.Add (new Vector4(c.x,c.y,v.x,v.y));
			}
		}
		// Try again but with the flipped dimensions (if not a square)
		// and if the space is close to corner
		if (space.y != space.x) {
			foreach (Vector2 c in checks) {
				List<Vector2> results = new List<Vector2> ();
				results = findSpaces (c, (int)space.y, (int)space.x, gridSizeX, gridSizeY, false, gridArray);
				foreach (Vector2 v in results) {
					possible.Add (new Vector4 (c.x, c.y, v.x, v.y));
				}
			}
		}
		return possible;
	}
	public void markSpot(Vector2 shape, Vector2 spawn, GameObject prefab, string name, bool mustBeAnchoredX, bool mustBeAnchoredY, GridSpace [,] gridArray, int gridSizeX, int gridSizeY){
		List<Vector2> temp = new List<Vector2> ();
		temp.Add (spawn);
		markSpot (shape, temp, prefab, name, mustBeAnchoredX, mustBeAnchoredY, gridArray, gridSizeX, gridSizeY);
		return;
	}
	public void markSpot(Vector2 shape, List<Vector2> checks, GameObject prefab, string name, bool mustBeAnchoredX, bool mustBeAnchoredY, GridSpace [,] gridArray, int gridSizeX, int gridSizeY){
		List<Vector4> possible = new List<Vector4>();
		possible = findPossibles(shape, checks, gridArray, gridSizeX, gridSizeY);
		int counter = 0;
		while (possible.Count == 0 && counter < 100) {
			possible = findPossibles(shape, checks, gridArray, gridSizeX, gridSizeY);
			counter++;
		}
		if (possible.Count == 0) {
			//Debug.Log ("Possible position list empty.");
			return;
		}
		Vector4 choice = possible [Random.Range (0, possible.Count)]; // Pick one possible at random
		//while loop here
		//loop thru the axiss in question and see if its all on a wall
		// if not try another possible
		//if none of them work kick it back to findPossibles
		//maybe have a counter so it can run too much
		// Anchored means every gridspace along specified axis is abutting a wall
		bool anchors = false; // Whether the anchor conditions are met
		counter = 0; // Stop trying to find valid positions 
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
				if (ylen != check) {
					anchors = false;
				}
			}
			if (choice.y == 0 || choice.y == gridSizeY - 1) {
				//its on a y wall and you need to check its width
				if (xlen != check) {
					anchors = false;
				}
			}
			if (!anchors) {
				// its not anchored remove it from the list and get a new one
				possible.Remove(choice);
				if (possible.Count != 0) {
					choice = possible [Random.Range (0, possible.Count)];
				} else {
					return;
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
				return;
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
				return;
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
				return;
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
			return;
		}
		return;
	}
	//end of class
}