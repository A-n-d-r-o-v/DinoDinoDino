using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {

	public GameObject meteorPrefab;
	public int meteorCount;
	public bool isSimulating;

	List<MovingMeteor> meteors = new List<MovingMeteor>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			if (meteorCount > 0) {
				Vector2 mousePos = Input.mousePosition;
				Vector2 finalMousePos = Camera.main.ScreenToWorldPoint (mousePos);
				meteors.Add(Instantiate (meteorPrefab, finalMousePos, Quaternion.identity).GetComponent<MovingMeteor>());
				meteorCount--;
			} else {
				foreach (MovingMeteor meteor in meteors) {
					meteor.isMoving = true;
				}
			}
		}

	}
}
