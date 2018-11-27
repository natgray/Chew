using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public GameController gamecontroller;
	public GridSpace[,] spaces; // an array to keep info about potential spaces in the room
	public int gridSizeX; // The size of the room
	public int gridSizeY;
	public int roomType;
	float halfX;
	float halfY;
	float wallHeight;
	public GameObject cubePrefab;
	List<GameObject> cubes = new List<GameObject>();
	List<Vector2> corners;

	// Use this for initialization
	void Start () {
		halfX = gridSizeX - 1;
		halfY = gridSizeY - 1;
		halfX /= 2;
		halfY /= 2;
		wallHeight = 0.3F;
		corners = new List<Vector2> ();
		corners.Add (new Vector2 (0, 0));
		corners.Add (new Vector2 (gridSizeX-1, 0));
		corners.Add (new Vector2 (0, gridSizeY-1));
		corners.Add (new Vector2 (gridSizeX-1, gridSizeY-1));
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				if (s.furniture.Equals ("bed")) {
					gameObject.AddComponent<BedroomGenerator> ();
					gameObject.GetComponent<BedroomGenerator> ().gamecontroller = this.gamecontroller;
				}
			}
		}
		spawnFloor ();
		spawnWalls ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnFurniture(){
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				GameObject furniture = Object.Instantiate (s.prefab, Vector3.zero, Quaternion.Euler(-90,0,0));
				furniture.transform.SetParent (gameObject.transform);
				if (s.furniture.Equals ("bed")) {
					int rotation = 0;
					// Decode rotation
					//Detect whether horizontal(0 or vertical
					bool horizontal = false;
					float xOffset, yOffset;
					xOffset = yOffset = 0;
					if ((Mathf.Abs (s.shape.x - s.shape.z)) > (Mathf.Abs (s.shape.y - s.shape.w))) {
						horizontal = true;
					}
					if (s.x > 0) {
						if (s.y > 0) {
							// +/+
							if (horizontal) {
								rotation = -90;
								xOffset = 0.7F;
								yOffset = 0.5F;
							} else {
								rotation = 180;
								xOffset = 0.5F;
								yOffset = 0.7F;
							}
						} else {
							// +/-
							if (horizontal) {
								rotation = -90;
								xOffset = 0.7F;
								yOffset = -0.5F;
							} else {
								rotation = 0;
								xOffset = 0.5F;
								yOffset = -0.7F;
							}
						}
					} else {
						if (s.y > 0) {
							// -/+
							if (horizontal) {
								rotation = 90;
								xOffset = -0.7F;
								yOffset = 0.5F;
							} else {
								rotation = 180;
								xOffset = -0.5F;
								yOffset = 0.7F;
							}
						} else {
							// -/-
							if (horizontal) {
								rotation = 90;
								xOffset = -0.7F;
								yOffset = -0.5F;
							} else {
								rotation = 0;
								xOffset = -0.5F;
								yOffset = -0.7F;
							}
						}

					}

					furniture.transform.localPosition = (new Vector3 (s.gridPos.x+xOffset, 1, s.gridPos.y+yOffset)); 
					furniture.transform.rotation = Quaternion.Euler (-90, 0, rotation);
				} else if(s.furniture.Equals("dresser")){
					int rotation = 0;
					// Decode rotation
					//Detect whether horizontal(0 or vertical
					bool horizontal = false;
					float xoffset, yoffset;
					xoffset = yoffset = 0F;
					if ((Mathf.Abs (s.shape.x - s.shape.z)) > (Mathf.Abs (s.shape.y - s.shape.w))) {
						horizontal = true;
					}
					if (horizontal) {
						if (s.gridPos.y == 0) {
							if (s.x > 0) {
								rotation = -180; //done
								xoffset = 0.5F; // done
							} else {
								rotation = -180; // done
								xoffset = -0.5F; // done 
							}
						} else {
							if (s.x > 0) {
								rotation = 0; // done
								xoffset = 0.5F; // done
							} else {
								rotation = 0; // done
								xoffset = -0.5F; //done
							}
						}
					} else {
						if (s.gridPos.x == 0) {
							if (s.y > 0) {
								rotation = -90; //done
								yoffset = 0.5F; //done
							} else {
								rotation = -90; //done
								yoffset = -0.5F; //done
							}
						} else {
							if (s.y > 0) {
								rotation = 90; // done
								yoffset = 0.5F; //done 
							} else {
								rotation = 90; // done
								yoffset = -0.5F; // done
							}
						}
					}
					furniture.transform.rotation = Quaternion.Euler (-90, rotation, 0);
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xoffset, 1, s.gridPos.y+yoffset)); 
				} else if(s.furniture.Equals("desk and chair")){
					int rotation = 0;
					// Decode rotation
					float xoffset, yoffset;
					xoffset = yoffset = 0F;
					if (s.gridPos.y == 0) {
						if (s.x > 0) {
							rotation = -90; //done
							xoffset = 0.5F; //done
							yoffset = 1.0F; //done
						} else {
							rotation = -90; // done
							xoffset = -0.5F; //done
							yoffset = 1.0F; //done
						}
					} else if (s.gridPos.y == gridSizeY - 1) {
						if (s.x > 0) {
							rotation = 90; //done
							xoffset = 0.5F; //done
							yoffset = -1.0F; //done
						} else {
							rotation = 90; //done
							xoffset = -0.5F; //done
							yoffset = -1.0F; //done
						}
					} else {
						if (s.gridPos.x == 0) {
							if (s.y > 0) {
								rotation = -90; //done
								yoffset = 1.0F; //done
								xoffset = 0.5F; //done
							} else {
								rotation = 60; 
								yoffset = 0.0F;
								Debug.Log (this.name + "has a 60");
							}
						} else {
							if (s.y > 0) {
								rotation = 180;  // done
								yoffset = 0.5F;  //done
								xoffset = -1.0F; //done
							} else {
								rotation = 180; //done
								xoffset = -1.0F; // done
								yoffset = -0.5F; //done
							}
						}
					}
					furniture.transform.rotation = Quaternion.Euler (-90, rotation, 0);
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xoffset, 1, s.gridPos.y + yoffset)); 
				} else if(s.furniture.Equals("bookshelf")){
					int rotation = 0;
					// Decode rotation
					float xoffset, yoffset;
					xoffset = yoffset = 0F;
					if(corners.Contains(s.gridPos)){
						if (s.gridPos.x == 0 && s.gridPos.y == 0) {
							rotation = -45;
						}
						if (s.gridPos.x == 0 && s.gridPos.y == gridSizeY-1) {
							rotation = 45;
						}
						if (s.gridPos.x == gridSizeX-1 && s.gridPos.y == 0) {
							rotation = 45;
						}
						if (s.gridPos.x == gridSizeX-1 &&  s.gridPos.y == gridSizeY-1) {
							rotation = -45;
						}
					}else if (s.gridPos.x == 0 || s.gridPos.x == gridSizeX - 1) {
						rotation = 0;
					} else {
						rotation = 90;
					}
					furniture.transform.rotation = Quaternion.Euler (-90, rotation, 0);
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xoffset, 1, s.gridPos.y + yoffset)); 
				}else if(s.furniture.Equals("floor lamp")){
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x, 1, s.gridPos.y));
					GameObject bulb = Object.Instantiate (gamecontroller.GetComponent<PreFabLibrary> ().bulb, Vector3.zero,Quaternion.Euler(-90,0,0));
					bulb.transform.SetParent (furniture.transform);
					bulb.transform.localPosition = (new Vector3 (0, 0, 1.3F));
				} else {
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x, 1, s.gridPos.y)); 
				}
			}
		}
	}

	public void DrawMap(){
		if(cubes.Count != 0)
		{
			foreach(GameObject c in cubes)
			{
				Destroy(c);
				cubes.Remove(c);
			}
		} 
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				GameObject cube = Object.Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3(s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.red;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add(cube);
			} else if (!s.walkway && !s.occupied) {
				GameObject cube = Object.Instantiate (cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.white;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add(cube);
			} else if(s.occupied && !s.walkway){
				GameObject cube = Object.Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3(s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer>().material.color = Color.magenta;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add(cube);
			}
			else {
				GameObject cube = Object.Instantiate (cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.cyan;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add(cube);
			}
		}
	}

	void spawnFloor(){
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.SetParent (gameObject.transform);
		floor.transform.localPosition = Vector3.zero;
		floor.transform.localScale = new Vector3 ((gridSizeX)*0.1F, 1, (gridSizeY)*0.1F);
		floor.transform.localPosition = new Vector3(halfX,0.99F,halfY);
		floor.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().flooring;
	}

	void spawnWalls(){
		//north wall is x0 to xMax, yMax 
		GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		northWall.transform.SetParent (gameObject.transform);
		northWall.transform.localScale = new Vector3 ((gridSizeX) * 0.1F, 1.0F, wallHeight);
		northWall.transform.rotation = Quaternion.Euler (-90, 0, 0);
		northWall.transform.localPosition = new Vector3(halfX,1.49F,(gridSizeY-0.5F));
		northWall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
		//east wall is xMax, y0 to yMax
		GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		eastWall.transform.SetParent (gameObject.transform);
		eastWall.transform.localScale = new Vector3 ((gridSizeX)*0.1F, 1.0F, wallHeight);
		eastWall.transform.rotation = Quaternion.Euler (-90, 0, 90);
		eastWall.transform.localPosition = new Vector3(gridSizeX-0.5F,1.49F,halfY);
		eastWall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
		// south wall is x0 to xMax, y0
		GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		southWall.transform.SetParent (gameObject.transform);
		southWall.transform.localScale = new Vector3 ((gridSizeX) * 0.1F, 1.0F, wallHeight);
		southWall.transform.rotation = Quaternion.Euler (90, 0, 0);
		southWall.transform.localPosition = new Vector3(halfX,1.49F,-0.5F);
		southWall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
		// west wall is x0, y0 to yMax
		GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		westWall.transform.SetParent (gameObject.transform);
		westWall.transform.localScale = new Vector3 ((gridSizeX)*0.1F, 1.0F, wallHeight);
		westWall.transform.rotation = Quaternion.Euler (-90, 0, -90);
		westWall.transform.localPosition = new Vector3(-0.5F,1.49F,halfY);
		westWall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
	}
	//end of class
}
