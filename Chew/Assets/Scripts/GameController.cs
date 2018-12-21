using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {
	List<GameObject> rooms;
	public List<GameObject> destructionSpots;
	List<Vector2> roomSlots;
	public List<NavMeshSurface> surfaces;
	public Canvas UI, GameOverUI;
	public GameObject PlayerRabbit, HumanAI;
	public PlayerController PlayerController;
	public bool gameOver, tipTimedOut, rabbitVictory;
	private float tipTimeOut;
	public TextMeshProUGUI bookCountText, gameOverText;

	// Use this for initialization
	void Awake () {
		UI.enabled = true;
		GameOverUI.enabled = false;
		gameOver = false;
		rabbitVictory = false;
		tipTimeOut = 5.0F;
		tipTimedOut = false;
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
		destructionSpots = new List<GameObject> ();
		for (int x = 0; x < 4; x++) {
			GameObject room = new GameObject ();
			room.AddComponent<Room> ();
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
		if (gameOver || GameOverUI.isActiveAndEnabled) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
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

	public void deregeisterDestructionSpot (GameObject g){
		destructionSpots.Remove (g);
	}
	// Update is called once per frame
	void Update () {
		if (destructionSpots.Count <= 0) {
			rabbitVictory = true;
			setGameOver ();
		}
		if (Input.GetKey ("f") && gameOver) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		}
		if (Input.GetKey ("q") && gameOver) {
			Debug.Log ("Quitting");
			Application.Quit ();
		}
		if(tipTimeOut <= 0){
			tipTimedOut = true;
		}
		if (!tipTimedOut) {
			tipTimeOut -= Time.deltaTime;
		} else {
			bookCountText.SetText ("Books remaining: " + destructionSpots.Count);
		}
	}

	public void setGameOver(){
		UI.enabled = false;
		GameOverUI.enabled = true;
		gameOver = true;
		if (rabbitVictory) {
			gameOverText.SetText ("You won! \n Press F to Restart \n Press Q to Quit");
			Destroy (HumanAI);
		} else {
			gameOverText.SetText ("Game Over :( \n Press F to Restart \n Press Q to Quit");
			PlayerRabbit.GetComponent<SkinnedMeshRenderer> ().enabled = false;
			PlayerController.speed = 0.0F;
		}
	}
}
