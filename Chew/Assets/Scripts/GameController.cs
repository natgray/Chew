using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {
	List<GameObject> rooms;
	public List<GameObject> destructionSpots;
	List<Vector2> roomSlots;
	public GameObject prefab;
	public List<NavMeshSurface> surfaces;
	public GameObject testSpot;

	// Use this for initialization
	void Awake () {
		Cursor.lockState = CursorLockMode.Confined;
		rooms = new List<GameObject> ();
		surfaces = new List<NavMeshSurface> ();
		gameObject.AddComponent<NavMeshSurface> ();
		regeisterNavMesh (gameObject.GetComponent<NavMeshSurface> ());
		roomSlots = new List<Vector2> ();
		roomSlots.Add (Vector2.zero);
		roomSlots.Add (new Vector2(0,1));
		roomSlots.Add (new Vector2(1,1));
		roomSlots.Add (new Vector2(1,0));
		regeisterDestructionSpot (testSpot);
		destructionSpots = new List<GameObject> ();
		for (int x = 0; x < 4; x++) {
			GameObject room = new GameObject ();
			room.AddComponent<Room> ();
			room.GetComponent<Room> ().cubePrefab = prefab;
			room.GetComponent<Room> ().gamecontroller = this;
			int len = Random.Range(4,9);
			int width = Random.Range(4,9);
			gameObject.GetComponent<RoomGenerator> ().GenerateRoom (len, width, room.transform);
			room.transform.position = new Vector3 (roomSlots[0].x*10,0,roomSlots[0].y*10); 
			roomSlots.RemoveAt (0);
			room.name = "Room #" + (x+1);
			//Debug.Log ("Finished Generating room #" + (x + 1));
			rooms.Add (room);
		} 	

		for (int i = 0; i < surfaces.Count; i++) {
			surfaces [i].BuildNavMesh ();
		}
	}

	public GameObject getRandomDestructionSpot(){
		if (destructionSpots.Count == 0) {
			Debug.Log ("No destruction Spots found");
			return null;
		}
		return destructionSpots [Random.Range (0, destructionSpots.Count)];
	}
	public void regeisterDestructionSpot (GameObject g){
		destructionSpots.Add (g);
	}

	public void regeisterNavMesh(NavMeshSurface surface){
		surfaces.Add (surface);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
