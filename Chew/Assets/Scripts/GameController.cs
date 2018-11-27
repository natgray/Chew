using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	List<GameObject> rooms;
	public GameObject prefab;
	// Use this for initialization
	void Start () {
		rooms = new List<GameObject> ();
		for (int x = 0; x < 10; x++) {
			GameObject room = new GameObject ();
			room.AddComponent<Room> ();
			room.GetComponent<Room> ().cubePrefab = prefab;
			room.GetComponent<Room> ().gamecontroller = this;
			gameObject.GetComponent<RoomGenerator> ().GenerateRoom (6, 6, room.transform);
			room.transform.position = new Vector3 (-50-(x*15), 0, 0); 
			room.name = "Room #" + (x+1);
			//Debug.Log ("Finished Generating room #" + (x + 1));
			rooms.Add (room);
		} 


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
