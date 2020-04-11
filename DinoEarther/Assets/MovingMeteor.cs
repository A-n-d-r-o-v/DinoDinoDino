using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMeteor : MonoBehaviour {

	public Vector2 velocity;
	public bool isMoving = false;

	public void setVelocity(Vector2 v){
		velocity = v;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			transform.Translate (velocity, Space.World);
		}
	}
}
