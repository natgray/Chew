using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreFabLibrary : MonoBehaviour {
	public GameObject bed, nightstand, cube, dresser, deskandchair, smalltable, chair;
	public GameObject floorlamp;
	public GameObject bulb;
	public Material woodOne, woodTwo, clothOne, clothTwo, plaster, flooring;
	public List<GameObject> bookshelves = new List<GameObject>();

	public GameObject getPrefab(string name){
		if (name.Equals ("Bed")) {
			return bed;
		} else if (name.Equals ("NightStand")) {
			return nightstand;
		} else if (name.Equals ("Dresser")) {
			return dresser;
		} else if (name.Equals ("Desk with Chair")) {
			return deskandchair;
		} else if (name.Equals ("Small Table")) {
			return smalltable;
		} else if (name.Equals ("Chair")) {
			return chair; 
		}else {
			return cube;
		}
	}

	public GameObject getBookShelf(){
		if (bookshelves.Count == 0) {
			return null;
		} 
		return bookshelves [Random.Range (0, bookshelves.Count)];
	}

}
