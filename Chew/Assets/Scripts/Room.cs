using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
	public GameController gamecontroller;
	public GridSpace[,] spaces;
	// an array to keep info about potential spaces in the room
	public int gridSizeX;
	// The size of the room
	public int gridSizeY;
	public int roomType;
	float halfX;
	float halfY;
	float wallHeight;
	public GameObject cubePrefab;
	List<GameObject> cubes = new List<GameObject> ();
	List<Vector2> corners;

	// Use this for initialization
	void Start ()
	{
		halfX = gridSizeX - 1;
		halfY = gridSizeY - 1;
		halfX /= 2;
		halfY /= 2;
		wallHeight = 0.3F;
		corners = new List<Vector2> ();
		corners.Add (new Vector2 (0, 0));
		corners.Add (new Vector2 (gridSizeX - 1, 0));
		corners.Add (new Vector2 (0, gridSizeY - 1));
		corners.Add (new Vector2 (gridSizeX - 1, gridSizeY - 1));
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				if (s.furniture.Equals ("bed")) {
					gameObject.AddComponent<BedroomGenerator> ();
					gameObject.GetComponent<BedroomGenerator> ().gamecontroller = this.gamecontroller;
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void SpawnFurniture ()
	{
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				GameObject furniture = Object.Instantiate (s.prefab, Vector3.zero, Quaternion.Euler (-90, 0, 0));
				furniture.transform.SetParent (gameObject.transform);
				furniture.AddComponent<NavMeshObstacle> ();
				furniture.GetComponent<NavMeshObstacle> ().carving = true;
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

					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xOffset, 1, s.gridPos.y + yOffset)); 
					furniture.transform.rotation = Quaternion.Euler (-90, 0, rotation);
				} else if (s.furniture.Equals ("dresser")) {
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
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xoffset, 1, s.gridPos.y + yoffset)); 
				} else if (s.furniture.Equals ("desk and chair")) {
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
				} else if (s.furniture.Equals ("bookshelf")) {
					int rotation = 0;
					// Decode rotation
					float xoffset, yoffset;
					xoffset = yoffset = 0F;
					if (corners.Contains (s.gridPos)) {
						if (s.gridPos.x == 0 && s.gridPos.y == 0) {
							rotation = -45;
						}
						if (s.gridPos.x == 0 && s.gridPos.y == gridSizeY - 1) {
							rotation = 45;
						}
						if (s.gridPos.x == gridSizeX - 1 && s.gridPos.y == 0) {
							rotation = 45;
						}
						if (s.gridPos.x == gridSizeX - 1 && s.gridPos.y == gridSizeY - 1) {
							rotation = -45;
						}
					} else if (s.gridPos.x == 0 || s.gridPos.x == gridSizeX - 1) {
						rotation = 0;
					} else {
						rotation = 90;
					}
					furniture.transform.rotation = Quaternion.Euler (-90, rotation, 0);
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x + xoffset, 1, s.gridPos.y + yoffset)); 
				} else if (s.furniture.Equals ("floor lamp")) {
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x, 1, s.gridPos.y));
					GameObject bulb = Object.Instantiate (gamecontroller.GetComponent<PreFabLibrary> ().bulb, Vector3.zero, Quaternion.Euler (-90, 0, 0));
					bulb.transform.SetParent (furniture.transform);
					bulb.transform.localPosition = (new Vector3 (0, 0, 1.3F));
				} else {
					furniture.transform.localPosition = (new Vector3 (s.gridPos.x, 1, s.gridPos.y)); 
				}
				if (s.attachedProps.Count != 0) {
					foreach (GameObject p in s.attachedProps) {
						GameObject prop = Object.Instantiate (p, Vector3.zero, Quaternion.Euler (0, 0, 0));
						prop.transform.SetParent (furniture.transform);
						if (p.Equals (gamecontroller.GetComponent<PreFabLibrary> ().propLight)) {
							prop.transform.rotation = Quaternion.Euler (-90, 0, 0);
							GameObject bulb = Object.Instantiate (gamecontroller.GetComponent<PreFabLibrary> ().bulb, Vector3.zero, Quaternion.Euler (-90, 0, 0));
							bulb.transform.SetParent (prop.transform);
							bulb.transform.localPosition = (new Vector3 (0, 0, 1.3F));
						}
						if (s.furniture.Equals ("nightstand")) {
							prop.transform.localPosition = (new Vector3 (0, 0, 0.8F));
						} else if (s.furniture.Equals ("dresser")) {
							prop.transform.localPosition = (new Vector3 (0, 0, 0.5F));
						} else {
							prop.transform.localPosition = (new Vector3 (0, 0, 0.5F));
						}
					}

				}
			} else {
				if (s.attachedProps.Count != 0) {
					foreach (GameObject p in s.attachedProps) {
						GameObject prop = Object.Instantiate (p, Vector3.zero, Quaternion.Euler (0, 0, -90));
						prop.transform.SetParent (gameObject.transform);
						prop.transform.localPosition = (new Vector3 (s.gridPos.x, 1F, s.gridPos.y));
						gamecontroller.regeisterDestructionSpot (prop);
						prop.tag = "chewable";
					}

				}
			}

		}
	}

	public void DrawMap ()
	{
		if (cubes.Count != 0) {
			foreach (GameObject c in cubes) {
				Destroy (c);
				cubes.Remove (c);
			}
		} 
		foreach (GridSpace s in spaces) {
			if (s.isPrime) {
				GameObject cube = Object.Instantiate (cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.red;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().doorway = s.doorway;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add (cube);
			} else if (s.doorway) {
				GameObject cube = Object.Instantiate (cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.green;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().doorway = s.doorway;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add (cube);
			}else if (!s.walkway && !s.occupied) {
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
				cube.GetComponent<TestCube> ().doorway = s.doorway;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add (cube);
			} else if (s.occupied && !s.walkway) {
				GameObject cube = Object.Instantiate (cubePrefab, Vector3.zero, Quaternion.identity);
				cube.transform.SetParent (gameObject.transform);
				cube.transform.localPosition = new Vector3 (s.gridPos.x, 0, s.gridPos.y);
				cube.GetComponent<Renderer> ().material.color = Color.magenta;
				cube.GetComponent<TestCube> ().gridPos = s.gridPos;
				cube.GetComponent<TestCube> ().shape = s.shape;
				cube.GetComponent<TestCube> ().prime = s.prime;
				cube.GetComponent<TestCube> ().prefab = s.prefab;
				cube.GetComponent<TestCube> ().isPrime = s.isPrime;
				cube.GetComponent<TestCube> ().walkway = s.walkway;
				cube.GetComponent<TestCube> ().walkstart = s.walkstart;
				cube.GetComponent<TestCube> ().occupied = s.occupied;
				cube.GetComponent<TestCube> ().doorway = s.doorway;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add (cube);
			} else {
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
				cube.GetComponent<TestCube> ().doorway = s.doorway;
				cube.GetComponent<TestCube> ().x = s.x;
				cube.GetComponent<TestCube> ().y = s.y;
				cubes.Add (cube);
			}
		}
	}

	public void spawnFloor ()
	{
		GameObject floor = GameObject.CreatePrimitive (PrimitiveType.Plane);
		floor.transform.SetParent (gameObject.transform);
		floor.transform.localPosition = Vector3.zero;
		floor.transform.localScale = new Vector3 ((gridSizeX) * 0.1F, 1, (gridSizeY) * 0.1F);
		floor.transform.localPosition = new Vector3 (halfX, 0.99F, halfY);
		floor.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().flooring;
		floor.AddComponent<NavMeshSurface> ();
		gamecontroller.regeisterNavMesh (floor.GetComponent<NavMeshSurface>());
	}

	public void spawnWalls ()
	{	
		foreach (GridSpace s in spaces) {
			if (s.gridPos.y == gridSizeY - 1) {
				// north wall
				GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Plane);
				GameObject wall2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall.transform.SetParent (gameObject.transform);
				wall2.transform.SetParent (gameObject.transform);
				wall.transform.rotation = Quaternion.Euler (-90, 0, 0);
				wall2.transform.rotation = Quaternion.Euler (-90, 180, 0);
				if (s.doorway) {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall.transform.localPosition = new Vector3 (s.gridPos.x, 2.775F, s.gridPos.y + 0.5F);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x, 2.775F, s.gridPos.y + 0.5F);
				} else {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall.transform.localPosition = new Vector3 (s.gridPos.x, 1.49F, s.gridPos.y + 0.5F);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x, 1.49F, s.gridPos.y + 0.5F);
					wall.AddComponent<NavMeshObstacle> ();
					wall.GetComponent<NavMeshObstacle> ().carving = true;
					wall.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
					wall2.AddComponent<NavMeshObstacle> ();
					wall2.GetComponent<NavMeshObstacle> ().carving = true;
					wall2.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
				}
				wall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
				wall2.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;

			}
			if (s.gridPos.y == 0) {
				// south wall
				GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall.transform.SetParent (gameObject.transform);
				wall.transform.rotation = Quaternion.Euler (90, 0, 0);
				GameObject wall2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall2.transform.SetParent (gameObject.transform);
				wall2.transform.rotation = Quaternion.Euler (90, 180, 0);
				if (s.doorway) {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall.transform.localPosition = new Vector3 (s.gridPos.x, 2.775F, s.gridPos.y - 0.5F);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x, 2.775F, s.gridPos.y - 0.5F);
				} else {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall.transform.localPosition = new Vector3 (s.gridPos.x, 1.49F, s.gridPos.y- 0.5F);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x, 1.49F, s.gridPos.y- 0.5F);
					wall.AddComponent<NavMeshObstacle> ();
					wall.GetComponent<NavMeshObstacle> ().carving = true;
					wall.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
					wall2.AddComponent<NavMeshObstacle> ();
					wall2.GetComponent<NavMeshObstacle> ().carving = true;
					wall2.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
				}
				wall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
				wall2.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
			}
			if (s.gridPos.x == gridSizeX - 1) {
				//east wall
				GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall.transform.SetParent (gameObject.transform);
				wall.transform.rotation = Quaternion.Euler (-90, 0, 90);
				GameObject wall2= GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall2.transform.SetParent (gameObject.transform);
				wall2.transform.rotation = Quaternion.Euler (-90, 180, 90);
				if (s.doorway) {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall.transform.localPosition = new Vector3 (s.gridPos.x + 0.5F, 2.775F, s.gridPos.y);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x + 0.5F, 2.775F, s.gridPos.y);
				} else {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall.transform.localPosition = new Vector3 (s.gridPos.x + 0.5F, 1.49F, s.gridPos.y);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x + 0.5F, 1.49F, s.gridPos.y);
					wall.AddComponent<NavMeshObstacle> ();
					wall.GetComponent<NavMeshObstacle> ().carving = true;
					wall.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
					wall2.AddComponent<NavMeshObstacle> ();
					wall2.GetComponent<NavMeshObstacle> ().carving = true;
					wall2.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
				}
				wall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
				wall2.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
			}
			if (s.gridPos.x == 0) {
				//west wall
				GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall.transform.SetParent (gameObject.transform);
				wall.transform.rotation = Quaternion.Euler (-90, 0, -90);
				GameObject wall2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
				wall2.transform.SetParent (gameObject.transform);
				wall2.transform.rotation = Quaternion.Euler (-90, 180, -90);
				if (s.doorway) {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall.transform.localPosition = new Vector3 (s.gridPos.x-0.5F, 2.775F, s.gridPos.y);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight/7);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x-0.5F, 2.775F, s.gridPos.y);
				} else {
					wall.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall.transform.localPosition = new Vector3 (s.gridPos.x-0.5F, 1.49F, s.gridPos.y);
					wall2.transform.localScale = new Vector3 (0.1F, 1.0F, wallHeight);
					wall2.transform.localPosition = new Vector3 (s.gridPos.x-0.5F, 1.49F, s.gridPos.y);
					wall.AddComponent<NavMeshObstacle> ();
					wall.GetComponent<NavMeshObstacle> ().carving = true;
					wall.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
					wall2.AddComponent<NavMeshObstacle> ();
					wall2.GetComponent<NavMeshObstacle> ().carving = true;
					wall2.GetComponent<NavMeshObstacle> ().size = new Vector3 (0.05F, 0, 10);
				}
				wall.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
				wall2.GetComponent<MeshRenderer> ().material = gamecontroller.GetComponent<PreFabLibrary> ().plaster;
			}
		}
	}
	//end of class
}
