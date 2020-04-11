using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

    public int maxLength;
	public float maxVelocity;
	public GameObject obj;
	public GameObject arrow;

	private Vector3 initialPos;

	void Start () {
		obj = Instantiate(obj);
		initialPos = transform.position;
	}

	void Update () {
		Vector2 mousePos = Input.mousePosition;
		Vector2 finalMousePos = Camera.main.ScreenToWorldPoint(mousePos) - initialPos;


		Vector2 endPoint = (Vector2)initialPos + Vector2.ClampMagnitude (finalMousePos, maxLength);
		obj.transform.position = endPoint;
		float angleDeg = Mathf.Rad2Deg * Mathf.Atan2 (obj.transform.position.y - transform.position.y, obj.transform.position.x - transform.position.x);
		transform.rotation = Quaternion.Euler (transform.rotation.x, transform.rotation.y, 
			angleDeg);

		arrow.transform.localScale = new Vector3(Vector2.Distance (transform.position, endPoint), 0.1f, 1f);

		if (Input.GetMouseButtonUp (0)) {
			float velocityMagnitude = maxVelocity * Mathf.Abs(arrow.transform.localScale.x) / maxLength;
			if (angleDeg < 0) {
				angleDeg += 360;
			}
			float angleRad = angleDeg * Mathf.Deg2Rad;
			GetComponent<MovingMeteor>().setVelocity(new Vector2(velocityMagnitude * Mathf.Cos(angleRad), 
				velocityMagnitude * Mathf.Sin(angleRad)));
			Destroy(obj);
			Destroy (arrow);
			Destroy (this);
		}
	}
}
